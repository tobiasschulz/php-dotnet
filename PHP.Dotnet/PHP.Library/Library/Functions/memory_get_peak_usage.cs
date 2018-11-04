using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class memory_get_peak_usage : ManagedFunction
    {
        public memory_get_peak_usage ()
            : base ("memory_get_peak_usage")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedCallParameter> parameters, FunctionScope function_scope)
        {
            return new Result (new LongExpression (12345));
        }
    }
}
