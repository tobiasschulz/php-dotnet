using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Library.Internal;
using PHP.Library.TypeSystem;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class ClassInterpreter
    {
        public static Result Run (ClassDeclarationExpression expression, Scope scope)
        {
            scope.Root.Classes.Add (new InterpretedClass (expression, scope));

            return Result.NULL;
        }

        private sealed class InterpretedClass : IClass
        {
            private readonly RootScope _rootscope;
            private readonly NameOfClass _name;
            private readonly ImmutableArray<NameOfClass> _parents;
            private readonly ImmutableArray<NameOfClass> _interfaces;
            private readonly IMethodCollection _methods;
            private readonly IFieldCollection _fields;
            private readonly IClassCollection _classes;
            private readonly IVariableCollection _static_variables;

            public InterpretedClass (ClassDeclarationExpression expression, Scope scope)
            {
                _rootscope = scope.Root;
                _name = expression.Name;
                _parents = expression.Parents;
                _interfaces = expression.Interfaces;
                _methods = new MethodCollection ();
                _fields = new FieldCollection ();
                _classes = new ClassCollection ();
                _static_variables = new VariableCollection ();

                ScriptScope script_scope = null;
                scope.FindNearestScope<ScriptScope> (ss => script_scope = ss);

                foreach (ClassMethodDeclarationExpression m in expression.Methods)
                {
                    _methods.Add (new InterpretedMethod (m, script_scope));
                }

                foreach (ClassFieldDeclarationExpression f in expression.Fields)
                {
                    IVariable var = _static_variables.EnsureExists (f.Name);
                    if (f.Initializer is Expression initializer_expr)
                    {
                        var.Value = Interpreters.Execute (initializer_expr, script_scope).ResultValue;
                    }
                }
            }

            NameOfClass IElement<NameOfClass>.Name => _name;
            IReadOnlyList<NameOfClass> IClass.Parents => _parents;
            IReadOnlyList<NameOfClass> IClass.Interfaces => _interfaces;
            IReadOnlyFieldCollection IClass.Fields => _fields;
            IReadOnlyMethodCollection IClass.Methods => _methods;
            IVariableCollection IClass.StaticVariables => _static_variables;

            public override string ToString ()
            {
                return $"[Class: {_name}]";
            }
        }

        private sealed class InterpretedMethod : IMethod
        {
            private readonly ClassMethodDeclarationExpression _expression;
            private readonly ScriptScope _script_scope;

            public InterpretedMethod (ClassMethodDeclarationExpression expression, ScriptScope script_scope)
            {
                _expression = expression;
                _script_scope = script_scope;
            }

            NameOfMethod IElement<NameOfMethod>.Name => _expression.Name;

            Result IMethod.Execute (IObject obj, IReadOnlyList<IClass> classes, EvaluatedSignature evaluated_signature, Scope outer_scope)
            {
                MethodScope function_scope = new MethodScope (outer_scope, this, evaluated_signature, obj, classes);

                foreach (EvaluatedParameter parameter in evaluated_signature.Parameters)
                {
                    if (!parameter.Name.IsEmpty)
                    {
                        IVariable variable = function_scope.Variables.EnsureExists (parameter.Name);
                        variable.Value = parameter.EvaluatedValue;
                    }
                }

                return Interpreters.Execute (_expression.Body, function_scope);
            }

            DeclarationSignature IMethod.DeclarationSignature
            {
                get => _expression.DeclarationSignature;
            }

            ScriptScope IMethod.GetDeclarationScope ()
            {
                return _script_scope;
            }

            public override string ToString ()
            {
                return $"[Method: {_expression.Name}]";
            }
        }

        public static Result Run (NewInstanceExpression expression, Scope scope)
        {
            NameOfClass class_name = Interpreters.Execute (expression.ClassName, scope).ResultValue.GetStringValue ();
            if (scope.Root.Classes.TryGetValue (class_name, out IClass type))
            {
                IObject obj = new InterpretedObject (type, scope.Root);
                scope.Root.Objects.Add (obj);

                return new Result (obj.AsExpression);
            }
            else
            {
                Log.Error ($"Class could not be found: {class_name}, scope: {scope}");
                Log.Error ($"  existing classes: {scope.Root.Classes.GetAll ().Select (f => f.Name).Join (", ")}");
                return Result.NULL;
            }
        }

        private sealed class InterpretedObject : IObject
        {
            private readonly ObjectId _id;
            private readonly IReadOnlyList<IClass> _classes;
            private readonly IVariableCollection _variables;
            private readonly IReadOnlyMethodCollection _methods;

            public InterpretedObject (IClass type, RootScope rootscope)
            {
                _id = ++ObjectExtensions.LastestObjectId;
                _classes = rootscope.Classes.GetWithParentClasses (type);
                _variables = new VariableCollection ();
                _methods = MethodCollection.FromClasses (_classes);
            }

            ObjectId IElement<ObjectId>.Name => _id;
            IReadOnlyList<IClass> IObject.Classes => _classes;
            IEnumerable<NameOfClass> IObject.ClassNames => GetClassNames ();
            IVariableCollection IObject.Variables => _variables;
            IReadOnlyMethodCollection IObject.Methods => _methods;
            FinalExpression IObject.AsExpression => new ObjectPointerExpression (this);

            public IEnumerable<NameOfClass> GetClassNames ()
            {
                return _classes.Select (f => f.Name);
            }

            public override string ToString ()
            {
                return $"[Object: {GetClassNames ().Join (",")}]";
            }
        }

        public static Result Run (MethodCallExpression expression, Scope scope)
        {
            FinalExpression obj_reference = Interpreters.Execute (expression.ObjectReference, scope).ResultValue;
            if (obj_reference is ObjectPointerExpression pointer)
            {
                if (pointer.Object is IObject obj)
                {
                    if (obj.Methods.TryGetValue (expression.Name, out IMethod method))
                    {
                        Log.Message ($"call method {method}");
                        try
                        {
                            return method.Execute (obj, obj.Classes, new EvaluatedSignature (expression.CallSignature, method.DeclarationSignature, scope), scope);
                        }
                        catch (ReturnException ex)
                        {
                            return new Result (ex.ReturnValue);
                        }
                    }
                    else
                    {
                        Log.Error ($"Method could not be found: {expression.Name}, object: {obj}, classes: {obj.ClassNames.Join (",")}, scope: {scope}");
                        Log.Error ($"  existing functions: {obj.Methods.GetAll ().Select (f => f.Name).Join (", ")}");
                        return Result.NULL;
                    }
                }
                else
                {
                    Log.Error ($"Object could not be found: {pointer.Object.Name}, scope: {scope}");
                    return Result.NULL;
                }
            }
            else
            {
                Log.Error ($"Method is not called on an object, but on: {obj_reference}, scope: {scope}");
                return Result.NULL;
            }
        }

        public static Result Run (StaticMethodCallExpression expression, Scope scope)
        {
            NameOfClass class_name = Interpreters.Execute (expression.ClassName, scope).ResultValue.GetStringValue ();
            if (scope.Root.Classes.TryGetValue (class_name, out IClass type))
            {
                IReadOnlyList<IClass> classes = scope.Root.Classes.GetWithParentClasses (type);
                IReadOnlyMethodCollection methods_of_type = MethodCollection.FromClasses (classes);

                IObject previous_scope_obj = null;
                IFunctionLikeScope function_like_scope = null;
                scope.FindNearestScope<IFunctionLikeScope> (s => function_like_scope = s);
                if (function_like_scope is MethodScope method_scope)
                {
                    previous_scope_obj = method_scope.Object;
                }

                bool types_match_with_previous_scope_obj = false;
                if (previous_scope_obj != null)
                {
                    ImmutableArray<NameOfClass> obj_class_names = previous_scope_obj.ClassNames.ToImmutableArray ();
                    if (obj_class_names.Contains (class_name))
                    {
                        types_match_with_previous_scope_obj = true;
                        Log.Debug ($"Static method call: target type is one of the object's types: class name = {class_name}, object types: {obj_class_names.Join (",")}");
                    }
                    else
                    {
                        Log.Debug ($"Static method call: target type is NOT one of the object's types: class name = {class_name}, object types: {obj_class_names.Join (",")}");
                    }
                }

                if (methods_of_type.TryGetValue (expression.Name, out IMethod method))
                {
                    try
                    {
                        Log.Message ($"call static method {method}");
                        IObject obj = types_match_with_previous_scope_obj ? previous_scope_obj : null;
                        return method.Execute (obj, classes, new EvaluatedSignature (expression.CallSignature, method.DeclarationSignature, scope), scope);
                    }
                    catch (ReturnException ex)
                    {
                        return new Result (ex.ReturnValue);
                    }
                }
                else
                {
                    Log.Error ($"Method could not be found: {expression.Name}, object: {previous_scope_obj}, classes: {previous_scope_obj.ClassNames.Join (",")}, scope: {scope}");
                    Log.Error ($"  existing functions: {previous_scope_obj.Methods.GetAll ().Select (f => f.Name).Join (", ")}");
                    return Result.NULL;
                }
            }
            else
            {
                Log.Error ($"Class could not be found: {expression.Name}, scope: {scope}");
                Log.Error ($"  existing classes: {scope.Root.Classes.GetAll ().Select (f => f.Name).Join (", ")}");
                return Result.NULL;
            }
        }

        public static Result Run (StaticFieldAccessExpression expression, Scope scope)
        {
            Result res = Result.NULL;
            Resolve (expression, scope, (var) =>
            {
                res = new Result (var.Value);
            });
            return res;
        }

        public static void Resolve (StaticFieldAccessExpression expression, Scope scope, Action<IVariable> action)
        {
            NameOfClass class_name = Interpreters.Execute (expression.ClassName, scope).ResultValue.GetStringValue ();
            if (scope.Root.Classes.TryGetValue (class_name, out IClass type))
            {
                NameOfVariable variable_name = expression.Name;
                IReadOnlyList<IClass> classes = scope.Root.Classes.GetWithParentClasses (type);
                IClass best_matching_class = classes.First ();
                foreach (IClass c in classes)
                {
                    if (c.StaticVariables.Contains (variable_name))
                        best_matching_class = c;
                }
                IVariable variable = best_matching_class.StaticVariables.EnsureExists (variable_name);
                action (variable);
            }
            else
            {
                Log.Error ($"Class could not be found: {expression.Name}, scope: {scope}");
                Log.Error ($"  existing classes: {scope.Root.Classes.GetAll ().Select (f => f.Name).Join (", ")}");
            }
        }

    }

}
