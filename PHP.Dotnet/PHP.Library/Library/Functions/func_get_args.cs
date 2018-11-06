using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Standard;
using PHP.Library.Internal;
using PHP.Tree;
using System.Linq;
using PHP.Library.TypeSystem;

namespace PHP.Library.Functions
{
    public sealed class func_get_args : ManagedFunction
    {
        public func_get_args ()
            : base ("func_get_args")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            IArray arr = new ArrayStructure ();
            function_scope.Root.Arrays.Add (arr);

            function_scope.Parent.FindNearestScope<IFunctionLikeScope> (s =>
            {
                foreach (var p in s.Signature.Parameters)
                {
                    arr.Set (new ArrayItem ("", p.EvaluatedValue));
                }
            });

            return new Result (arr.AsExpression);
        }
    }
}
