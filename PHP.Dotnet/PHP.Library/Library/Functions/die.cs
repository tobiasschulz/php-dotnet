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

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            foreach (EvaluatedParameter p in parameters)
            {
                function_scope.Root.Context.Console.Err.Write (p.EvaluatedValue.GetStringValue ());
            }

            return new Result (new NullExpression ()).DoFastReturn ();
        }
    }
}
