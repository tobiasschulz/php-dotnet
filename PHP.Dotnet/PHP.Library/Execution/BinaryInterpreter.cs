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
                    return new Result (new DoubleExpression (left.GetDoubleValue () / right.GetDoubleValue ()));

                case BinaryOp.CONCAT:
                    Log.Debug ($"concat result: left = " + left + ", right = " + right + ", concat = " + new StringExpression (left.GetStringValue () + right.GetStringValue ()));
                    return new Result (new StringExpression (left.GetStringValue () + right.GetStringValue ()));

                case BinaryOp.EQUAL:
                    if (left.GetScalarAffinity () == ScalarAffinity.ARRAY && right.GetScalarAffinity () == ScalarAffinity.ARRAY)
                        return new Result (new BoolExpression ((left as ArrayPointerExpression)?.Array?.Name == (right as ArrayPointerExpression)?.Array?.Name));
                    else if (left.GetScalarAffinity () == ScalarAffinity.OBJECT && right.GetScalarAffinity () == ScalarAffinity.OBJECT)
                        return new Result (new BoolExpression ((left as ObjectPointerExpression)?.Object?.Name == (right as ObjectPointerExpression)?.Object?.Name));
                    else if (left.GetScalarAffinity () == ScalarAffinity.BOOL || right.GetScalarAffinity () == ScalarAffinity.BOOL)
                        return new Result (new BoolExpression (left.GetBoolValue () == right.GetBoolValue ()));
                    else if (left.GetScalarAffinity () == ScalarAffinity.DOUBLE || right.GetScalarAffinity () == ScalarAffinity.DOUBLE)
                        return new Result (new BoolExpression (left.GetDoubleValue () == right.GetDoubleValue ()));
                    else if (left.GetScalarAffinity () == ScalarAffinity.LONG || right.GetScalarAffinity () == ScalarAffinity.LONG)
                        return new Result (new BoolExpression (left.GetLongValue () == right.GetLongValue ()));
                    else
                        return new Result (new BoolExpression (left.GetStringValue () == right.GetStringValue ()));

                case BinaryOp.IDENTICAL:
                    if (left.GetScalarAffinity () == ScalarAffinity.ARRAY && right.GetScalarAffinity () == ScalarAffinity.ARRAY)
                        return new Result (new BoolExpression ((left as ArrayPointerExpression)?.Array?.Name == (right as ArrayPointerExpression)?.Array?.Name));
                    else if (left.GetScalarAffinity () == ScalarAffinity.OBJECT && right.GetScalarAffinity () == ScalarAffinity.OBJECT)
                        return new Result (new BoolExpression ((left as ObjectPointerExpression)?.Object?.Name == (right as ObjectPointerExpression)?.Object?.Name));
                    else if (left.GetScalarAffinity () == ScalarAffinity.BOOL && right.GetScalarAffinity () == ScalarAffinity.BOOL)
                        return new Result (new BoolExpression (left.GetBoolValue () == right.GetBoolValue ()));
                    else if (left.GetScalarAffinity () == ScalarAffinity.DOUBLE && right.GetScalarAffinity () == ScalarAffinity.DOUBLE)
                        return new Result (new BoolExpression (left.GetDoubleValue () == right.GetDoubleValue ()));
                    else if (left.GetScalarAffinity () == ScalarAffinity.LONG && right.GetScalarAffinity () == ScalarAffinity.LONG)
                        return new Result (new BoolExpression (left.GetLongValue () == right.GetLongValue ()));
                    else if (left.GetScalarAffinity () == ScalarAffinity.NULL && right.GetScalarAffinity () == ScalarAffinity.NULL)
                        return new Result (new BoolExpression (left.GetLongValue () == right.GetLongValue ()));
                    else if (left.GetScalarAffinity () == ScalarAffinity.STRING && right.GetScalarAffinity () == ScalarAffinity.STRING)
                        return new Result (new BoolExpression (left.GetStringValue () == right.GetStringValue ()));
                    else
                        return new Result (new BoolExpression (false));

                case BinaryOp.LESS_THAN:
                    if (left.GetScalarAffinity () == ScalarAffinity.DOUBLE || right.GetScalarAffinity () == ScalarAffinity.DOUBLE)
                        return new Result (new BoolExpression (left.GetDoubleValue () < right.GetDoubleValue ()));
                    else if (left.GetScalarAffinity () == ScalarAffinity.LONG || right.GetScalarAffinity () == ScalarAffinity.LONG)
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
