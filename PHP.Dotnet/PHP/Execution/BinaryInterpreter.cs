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
            Expression left = expression.Left;
            Expression right = expression.Right;

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
