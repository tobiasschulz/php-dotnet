using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class error_reporting : ManagedFunction
    {
        public error_reporting ()
            : base ("error_reporting")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            return Result.NULL;
        }
    }
}
