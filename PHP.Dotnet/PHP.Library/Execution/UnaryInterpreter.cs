using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class UnaryInterpreter
    {
        public static Result Run (UnaryExpression expression, Scope scope)
        {
            Result value_result = Interpreters.Execute (expression.Value, scope);
            if (value_result.FastReturn)
            {
                return value_result;
            }

            FinalExpression value = value_result.ResultValue;

            switch (expression.Operation)
            {
                case UnaryOp.CAST_STRING:
                    return new Result (new StringExpression (value.GetStringValue ()));

                case UnaryOp.LOGICAL_NEGATION:
                    return new Result (new BoolExpression (!value.GetBoolValue ()));

                case UnaryOp.IS_EMPTY:
                    return new Result (new BoolExpression (string.IsNullOrEmpty (value.GetStringValue ())));

                case UnaryOp.AT_SIGN:
                    return new Result (value);

                case UnaryOp.ARRAY_CAST:
                    Result res = ArrayInterpreter.Run (new ArrayCreateExpression (ImmutableArray<ArrayItemExpression>.Empty), scope);
                    return res;

                default:
                    Log.Error ($"Unable to execute binary operation: {expression.Operation}");
                    return Result.NULL;
            }
        }
    }


}
