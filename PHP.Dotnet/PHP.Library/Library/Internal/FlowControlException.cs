using System;
using System.Collections.Generic;
using System.Text;
using PHP.Tree;

namespace PHP.Library.Internal
{
    public abstract class FlowControlException : InternalException
    {
        protected FlowControlException (string message)
            : base (message)
        {
        }
    }

    public sealed class ContinueException : FlowControlException
    {
        public readonly long CountOfLoops;

        public ContinueException (long count_of_loops)
            : base ($"continue: {count_of_loops}")
        {
            CountOfLoops = count_of_loops;
        }
    }

    public sealed class BreakException : FlowControlException
    {
        public readonly long CountOfLoops;

        public BreakException (long count_of_loops)
            : base ($"break: {count_of_loops}")
        {
            CountOfLoops = count_of_loops;
        }
    }

    public sealed class ReturnException : FlowControlException
    {
        public readonly FinalExpression ReturnValue;

        public ReturnException (FinalExpression return_value)
            : base ($"return: {return_value}")
        {
            ReturnValue = return_value;
        }
    }

}
