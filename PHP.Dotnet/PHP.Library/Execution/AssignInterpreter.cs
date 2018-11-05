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
                case ArrayAccessExpression array_access_expression:
                    ArrayInterpreter.Resolve (array_access_expression, scope, (arr, key) =>
                    {
                        arr.Set (new ArrayItem (key, right_result.ResultValue));
                    });
                    break;

                case VariableExpression variable_expression:
                    IVariable variable = scope.Variables.EnsureExists (variable_expression.Name);
                    variable.Value = right_result.ResultValue;
                    break;

                case StaticFieldAccessExpression static_field_access_expression:
                    ClassInterpreter.Resolve (static_field_access_expression, scope, (var) =>
                    {
                        var.Value = right_result.ResultValue;
                    });
                    break;

                default:
                    Log.Error ($"Cannot execute assign expression: Left Value is of unknown type {assign.Left}");
                    break;
            }

            return right_result;
        }

        public static Result Run (UnsetExpression unset, Scope scope)
        {
            foreach (Expression expr in unset.Variables)
            {
                switch (expr)
                {
                    case ArrayAccessExpression array_access_expression:
                        ArrayInterpreter.Resolve (array_access_expression, scope, (arr, key) =>
                        {
                            arr.Set (new ArrayItem (key, new NullExpression ()));
                        });
                        break;

                    case VariableExpression variable_expression:
                        IVariable variable = scope.Variables.EnsureExists (variable_expression.Name);
                        variable.Value = new NullExpression ();
                        break;

                    default:
                        Log.Error ($"Cannot execute unset expression: Left Value is of unknown type {expr}");
                        break;
                }
            }

            return Result.NULL;
        }
    }


}
