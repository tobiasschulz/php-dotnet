using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class FunctionDeclarationInterpreter
    {
        public static Result Run (FunctionDeclarationExpression expression, Scope scope)
        {
            scope.Root.GlobalFunctions.Add (new DeclaredFunction (expression));

            return Result.NULL;
        }

        private sealed class DeclaredFunction : IFunction
        {
            private readonly FunctionDeclarationExpression _expression;

            public DeclaredFunction (FunctionDeclarationExpression expression)
            {
                _expression = expression;
            }

            Name IFunction.Name => _expression.Name;

            Result IFunction.Execute (EvaluatedCallSignature call_signature, Scope outer_scope)
            {
                FunctionScope function_scope = new FunctionScope (outer_scope, this);

                foreach ((DeclarationParameter decl_parameter, int i) in _expression.DeclarationSignature.Parameters.Select ((o, i) => (o, i)))
                {
                    EvaluatedCallParameter call_parameter = i < call_signature.Parameters.Length ? call_signature.Parameters [i] : null;
                    Expression parameter_expression = decl_parameter.InitialValue;
                    if (call_parameter != null)
                    {
                        parameter_expression = call_parameter.EvaluatedValue;
                    }
                    if (parameter_expression != null)
                    {
                        function_scope.Variables.EnsureExists (decl_parameter.Name, out IVariable variable);
                        variable.Value = Interpreters.Execute (parameter_expression, outer_scope).ResultValue;
                    }
                }

                return Interpreters.Execute (_expression.Body, function_scope);
            }
        }
    }

}
