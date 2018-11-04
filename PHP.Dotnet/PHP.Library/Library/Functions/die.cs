using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class die : ManagedFunction
    {
        public die ()
            : base ("die")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedCallParameter> parameters, FunctionScope function_scope)
        {
            foreach (EvaluatedCallParameter p in parameters)
            {
                function_scope.Root.Context.Console.Err.Write (p.EvaluatedValue.GetStringValue ());
            }

            return new Result (new NullExpression ()).DoFastReturn ();
        }
    }
}
