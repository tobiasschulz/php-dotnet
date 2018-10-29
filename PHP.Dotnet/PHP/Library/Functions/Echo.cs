using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class Echo : Function
    {
        public Echo ()
            : base ("echo")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedCallParameter> parameters, FunctionScope function_scope)
        {
            foreach (EvaluatedCallParameter p in parameters)
            {
                function_scope.Root.Context.Console.Out.Write (p.EvaluatedValue.GetStringValue ());
            }

            return Result.NULL;
        }
    }
}
