using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class mb_substr : ManagedFunction
    {
        public mb_substr ()
            : base ("mb_substr")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            return new Result (new StringExpression ("not implemented"));
        }
    }
}
