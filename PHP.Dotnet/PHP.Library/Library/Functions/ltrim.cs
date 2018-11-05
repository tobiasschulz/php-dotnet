using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class ltrim : ManagedFunction
    {
        public ltrim ()
            : base ("ltrim")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 1 && parameters.Length != 2)
            {
                throw new WrongParameterCountException (this, expected: 1, actual: parameters.Length);
            }

            string path = parameters [0].EvaluatedValue.GetStringValue ();
            string mask = parameters.Length >= 2 ? parameters [1].EvaluatedValue.GetStringValue () : null;

            if (!string.IsNullOrEmpty (mask))
            {
                path = path.TrimStart (mask.ToCharArray ());
            }
            else
            {
                path = path.TrimStart ();
            }

            return new Result (new StringExpression (path));
        }
    }
}
