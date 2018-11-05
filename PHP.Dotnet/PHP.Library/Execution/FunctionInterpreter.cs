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
    public static class FunctionInterpreter
    {
        public static Result Run (FunctionCallExpression expression, Scope scope)
        {
            if (scope.Root.Functions.TryGetValue (expression.Name, out IFunction function))
            {
                try
                {
                    return function.Execute (new EvaluatedSignature (expression.CallSignature, function.DeclarationSignature, scope), scope);
                }
                catch (ReturnException ex)
                {
                    return new Result (ex.ReturnValue);
                }
            }
            else
            {
                Log.Error ($"Function could not be found: {expression.Name}, scope: {scope}");
                Log.Error ($"  existing functions: {scope.Root.Functions.GetAll ().Select (f => f.Name).Join (", ")}");
                return Result.NULL;
            }
        }

        public static Result Run (FunctionDeclarationExpression expression, Scope scope)
        {
            ScriptScope script_scope = null;
            scope.FindNearestScope<ScriptScope> (ss => script_scope = ss);

            scope.Root.Functions.Add (new InterpretedFunction (expression, script_scope));

            return Result.NULL;
        }

        private sealed class InterpretedFunction : IFunction
        {
            private readonly FunctionDeclarationExpression _expression;
            private readonly ScriptScope _script_scope;

            public InterpretedFunction (FunctionDeclarationExpression expression, ScriptScope script_scope)
            {
                _expression = expression;
                _script_scope = script_scope;
            }

            NameOfFunction IFunction.Name => _expression.Name;

            Result IFunction.Execute (EvaluatedSignature evaluated_signature, Scope outer_scope)
            {
                FunctionScope function_scope = new FunctionScope (outer_scope, this, evaluated_signature);

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

            DeclarationSignature IFunction.DeclarationSignature
            {
                get => _expression.DeclarationSignature;
            }

            ScriptScope IFunction.GetDeclarationScope ()
            {
                return _script_scope;
            }

            public override string ToString ()
            {
                return $"[Function: {_expression.Name}]";
            }
        }
    }

}
