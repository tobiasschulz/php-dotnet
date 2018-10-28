using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class BinaryInterpreter
    {
        public static Result Run (BinaryExpression expression, Scope scope)
        {
            Result left_result = Interpreters.Execute (expression.Left, scope);
            if (left_result.FastReturn)
            {
                return left_result;
            }
            Result right_result = Interpreters.Execute (expression.Right, scope);
            if (right_result.FastReturn)
            {
                return right_result;
            }

            FinalExpression left = left_result.ResultValue;
            FinalExpression right = right_result.ResultValue;

            switch (expression.Operation)
            {
                case BinaryOp.ADD:
                    if (left.GetScalarAffinity () == ScalarAffinity.DOUBLE || left.GetScalarAffinity () == ScalarAffinity.DOUBLE)
                        return new Result (new DoubleExpression (left.GetDoubleValue () + right.GetDoubleValue ()));
                    else
                        return new Result (new LongExpression (left.GetLongValue () + right.GetLongValue ()));

                case BinaryOp.SUB:
                    if (left.GetScalarAffinity () == ScalarAffinity.DOUBLE || left.GetScalarAffinity () == ScalarAffinity.DOUBLE)
                        return new Result (new DoubleExpression (left.GetDoubleValue () - right.GetDoubleValue ()));
                    else
                        return new Result (new LongExpression (left.GetLongValue () - right.GetLongValue ()));

                case BinaryOp.MUL:
                    if (left.GetScalarAffinity () == ScalarAffinity.DOUBLE || left.GetScalarAffinity () == ScalarAffinity.DOUBLE)
                        return new Result (new DoubleExpression (left.GetDoubleValue () * right.GetDoubleValue ()));
                    else
                        return new Result (new LongExpression (left.GetLongValue () * right.GetLongValue ()));

                case BinaryOp.DIV:
                    if (left.GetScalarAffinity () == ScalarAffinity.DOUBLE || left.GetScalarAffinity () == ScalarAffinity.DOUBLE)
                        return new Result (new DoubleExpression (left.GetDoubleValue () / right.GetDoubleValue ()));
                    else
                        return new Result (new LongExpression (left.GetLongValue () / right.GetLongValue ()));

                case BinaryOp.CONCAT:
                    Log.Debug ($"concat result: left = " + left + ", right = " + right + ", concat = " + new StringExpression (left.GetStringValue () + right.GetStringValue ()));
                    return new Result (new StringExpression (left.GetStringValue () + right.GetStringValue ()));

                case BinaryOp.EQUAL:
                    if (left.GetScalarAffinity () == ScalarAffinity.BOOL || left.GetScalarAffinity () == ScalarAffinity.BOOL)
                        return new Result (new BoolExpression (left.GetBoolValue () == right.GetBoolValue ()));
                    else if (left.GetScalarAffinity () == ScalarAffinity.DOUBLE || left.GetScalarAffinity () == ScalarAffinity.DOUBLE)
                        return new Result (new BoolExpression (left.GetDoubleValue () == right.GetDoubleValue ()));
                    else if (left.GetScalarAffinity () == ScalarAffinity.LONG || left.GetScalarAffinity () == ScalarAffinity.LONG)
                        return new Result (new BoolExpression (left.GetLongValue () == right.GetLongValue ()));
                    else
                        return new Result (new BoolExpression (left.GetStringValue () == right.GetStringValue ()));

                case BinaryOp.LESS_THAN:
                    if (left.GetScalarAffinity () == ScalarAffinity.DOUBLE || left.GetScalarAffinity () == ScalarAffinity.DOUBLE)
                        return new Result (new BoolExpression (left.GetDoubleValue () < right.GetDoubleValue ()));
                    else if (left.GetScalarAffinity () == ScalarAffinity.LONG || left.GetScalarAffinity () == ScalarAffinity.LONG)
                        return new Result (new BoolExpression (left.GetLongValue () < right.GetLongValue ()));
                    else
                        return new Result (new BoolExpression (left.GetStringValue ().CompareTo (right.GetStringValue ()) < 0));

                case BinaryOp.AND:
                    return new Result (new BoolExpression (left.GetBoolValue () && right.GetBoolValue ()));

                case BinaryOp.OR:
                    return new Result (new BoolExpression (left.GetBoolValue () || right.GetBoolValue ()));

                default:
                    Log.Error ($"Unable to execute binary operation: {expression.Operation}");
                    return Result.NULL;
            }
        }
    }


}
