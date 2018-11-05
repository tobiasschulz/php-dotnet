using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class microtime : ManagedFunction
    {
        public microtime ()
            : base ("microtime")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            long ticks = DateTime.UtcNow.Ticks;
            long microseconds = ticks / (TimeSpan.TicksPerMillisecond / 1000);

            return new Result (new LongExpression (microseconds));
        }
    }
}
