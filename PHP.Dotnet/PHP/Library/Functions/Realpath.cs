using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class Realpath : Function
    {
        public Realpath ()
            : base ("realpath")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedCallParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 1)
            {
                throw new WrongParameterCountException (this, expected: 1, actual: parameters.Length);
            }

            string path = parameters [0].EvaluatedValue.GetStringValue ();

            return new Result (new StringExpression (path));
        }
    }
}
