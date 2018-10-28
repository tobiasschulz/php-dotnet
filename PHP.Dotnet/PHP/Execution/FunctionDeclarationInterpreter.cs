using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
                return Interpreters.Execute (_expression.Body, function_scope);
            }
        }
    }

}
