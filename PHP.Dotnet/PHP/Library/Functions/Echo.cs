using System;
using System.Collections.Generic;
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

        protected override Result _execute (CallSignature call_signature, FunctionScope function_scope)
        {
            foreach (CallParameter p in call_signature.Parameters)
            {
                Result param_result = Interpreters.Execute (p.Value, function_scope);
                function_scope.Root.Context.Console.Out.Write (param_result.ResultValue.GetStringValue ());
            }

            return Result.NULL;
        }
    }
}
