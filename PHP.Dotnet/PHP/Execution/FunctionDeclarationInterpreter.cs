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

        private sealed class DeclaredFunction : IFunctionDeclaration
        {
            private readonly FunctionDeclarationExpression _expression;

            public DeclaredFunction (FunctionDeclarationExpression expression)
            {
                _expression = expression;
            }

            Name IFunctionDeclaration.Name => _expression.Name;

            Result IFunctionDeclaration.Execute (CallSignature call_signature, Scope scope)
            {
                FunctionScope function_scope = new FunctionScope (scope, this);

                foreach ((DeclarationParameter decl_parameter, int i) in _expression.DeclarationSignature.Parameters.Select ((o, i) => (o, i)))
                {
                    CallParameter call_parameter = i < call_signature.Parameters.Length ? call_signature.Parameters [i] : null;
                    Expression parameter_expression = decl_parameter.InitialValue;
                    if (call_parameter != null)
                    {
                        parameter_expression = call_parameter.Value;
                    }
                    if (parameter_expression != null)
                    {

                    }
                }

                return Interpreters.Execute (_expression.Body, function_scope);
            }
        }
    }

}
