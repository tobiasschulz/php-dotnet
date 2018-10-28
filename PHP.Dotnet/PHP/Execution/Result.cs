using System;
using System.Collections.Generic;
using System.Text;
using PHP.Tree;

namespace PHP.Execution
{
    public sealed class Result
    {
        public static readonly Result NULL = new Result (result_value: new NullExpression ());

        public readonly Expression ResultValue;
        public readonly bool FastReturn;

        public Result (Expression result_value)
            : this (result_value, fast_return: false)
        {
        }

        private Result (Expression result_value, bool fast_return)
        {
            ResultValue = result_value;
            FastReturn = fast_return;
        }

        public Result DoFastReturn ()
        {
            return new Result (
                result_value: ResultValue,
                fast_return: true
            );
        }

        public bool IsTrue ()
        {
            Log.Debug ($"is true? {ResultValue}");
            return ResultValue.GetBoolValue ();
        }
    }
}
