﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class strripos : ManagedFunction
    {
        public strripos ()
            : base ("strripos")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 2 && parameters.Length != 3)
            {
                throw new WrongParameterCountException (this, expected: 2, actual: parameters.Length);
            }

            string haystack = parameters [0].EvaluatedValue.GetStringValue ();
            string needle = parameters [1].EvaluatedValue.GetStringValue ();
            long startindex = parameters.Length >= 3 ? parameters [2].EvaluatedValue.GetLongValue () : 0;

            int index = haystack.LastIndexOf (needle, (int)startindex, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                return new Result (new BoolExpression (false));
            }

            return new Result (new LongExpression (index));
        }
    }
}
