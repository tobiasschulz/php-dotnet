using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Library.TypeSystem;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class ClassDeclarationInterpreter
    {
        public static Result Run (ClassDeclarationExpression expression, Scope scope)
        {
            scope.Root.Classes.Add (new InterpretedClass (expression, scope));

            return Result.NULL;
        }

        private sealed class InterpretedClass : IClass
        {
            private readonly NameOfClass _name;
            private readonly ImmutableArray<NameOfClass> _parents;
            private readonly ImmutableArray<NameOfClass> _interfaces;
            private readonly IMethodCollection _methods;
            private readonly IFieldCollection _fields;
            private readonly IClassCollection _classes;
            private readonly RootScope _rootscope;

            public InterpretedClass (ClassDeclarationExpression expression, Scope scope)
            {
                _name = expression.Name;
                _parents = expression.Parents;
                _interfaces = expression.Interfaces;
                _methods = new MethodCollection ();
                _fields = new FieldCollection ();
                _classes = new ClassCollection ();
                _rootscope = scope.Root;
            }

            NameOfClass IElement<NameOfClass>.Name => _name;
            IReadOnlyList<NameOfClass> IClass.Parents => _parents;
            IReadOnlyList<NameOfClass> IClass.Interfaces => _interfaces;
            IFieldCollection IClass.Fields => _fields;
            IMethodCollection IClass.Methods => _methods;
        }

        private sealed class InterpretedObject : IObject
        {
            private readonly ObjectId _id;
            private readonly IReadOnlyList<IClass> _classes;
            private readonly IVariableCollection _variables;

            public InterpretedObject (IClass type, RootScope rootscope)
            {
                _id = ++ObjectExtensions.LastestObjectId;
                _classes = _getAllClasses (type, rootscope);
                _variables = new VariableCollection ();
            }

            ObjectId IElement<ObjectId>.Name => _id;
            IReadOnlyList<IClass> IObject.Classes => _classes;
            IVariableCollection IObject.Variables => _variables;

            public static IObject CreateInstance (IClass type, RootScope rootscope)
            {
                return new InterpretedObject (type, rootscope);
            }

            private static IReadOnlyList<IClass> _getAllClasses (IClass type, RootScope rootscope)
            {
                Dictionary<NameOfClass, IClass> classes = new Dictionary<NameOfClass, IClass>
                {
                    [type.Name] = type,
                };
                List<NameOfClass> next = type.Parents.Concat (type.Interfaces).ToList ();
                do
                {
                    IReadOnlyList<NameOfClass> current = next.ToImmutableArray ();
                    next.Clear ();
                    foreach (var parent_name in current)
                    {
                        if (!classes.ContainsKey (parent_name) && rootscope.Classes.TryGetValue (parent_name, out IClass parent))
                        {
                            classes [parent_name] = parent;
                            next.AddRange (parent.Parents);
                            next.AddRange (parent.Interfaces);
                        }
                    }
                } while (next.Count != 0);
                return classes.Values.ToImmutableArray ();
            }
        }
    }

}
