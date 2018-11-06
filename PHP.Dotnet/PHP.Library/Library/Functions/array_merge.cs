using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Library.TypeSystem;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class array_merge : ManagedFunction
    {
        public array_merge ()
            : base ("array_merge")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            IArray result_array = new ArrayStructure ();
            function_scope.Root.Arrays.Add (result_array);

            foreach (EvaluatedParameter param in parameters)
            {
                if (param.EvaluatedValue is ArrayPointerExpression array_pointer)
                {
                    foreach (ArrayItem item in array_pointer.Array.GetAll ())
                    {
                        if (item.Key.IsDigitsOnly ())
                        {
                            result_array.Set (new ArrayItem ("", item.Value));
                        }
                        else
                        {
                            result_array.Set (new ArrayItem (item.Key, item.Value));
                        }
                    }
                }
            }

            return new Result (result_array.AsExpression);
        }
    }
}
