using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Library.TypeSystem;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class AssignInterpreter
    {
        public static Result Run (AssignExpression assign, Scope scope)
        {
            Result right_result = Interpreters.Execute (assign.Right, scope);

            if (right_result.FastReturn)
            {
                return right_result;
            }

            switch (assign.Left)
            {
                case ArrayExpression array_expression:
                    Log.Error ($"Cannot execute assign expression: Not Implemented: {assign.Left}");
                    break;

                case VariableExpression variable_expression:
                    scope.Variables.EnsureExists (variable_expression.Name, out IVariable variable);
                    variable.Value = right_result.ResultValue;
                    break;

                default:
                    Log.Error ($"Cannot execute assign expression: Left Value is of unknown type {assign.Left}");
                    break;
            }

            return right_result;
        }
    }


}
