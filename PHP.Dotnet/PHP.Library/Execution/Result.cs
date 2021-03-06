﻿using System;
using System.Collections.Generic;
using System.Text;
using PHP.Tree;

namespace PHP.Execution
{
    public sealed class Result
    {
        public static readonly Result NULL = new Result (result_value: new NullExpression ());

        public readonly FinalExpression ResultValue;
        public readonly bool FastReturn;

        public Result (FinalExpression result_value)
            : this (result_value, fast_return: false)
        {
        }

        private Result (FinalExpression result_value, bool fast_return)
        {
            ResultValue = result_value ?? new EmptyExpression ();
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
            // Log.Debug ($"is true? {ResultValue} {Environment.StackTrace}");
            Log.Debug ($"Check for truth: {ResultValue}");
            return ResultValue.GetBoolValue ();
        }
    }
}
