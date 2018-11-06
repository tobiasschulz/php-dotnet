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
    public sealed class array_shift : ManagedFunction
    {
        public array_shift ()
            : base ("array_shift")
        {
        }

        protected override IEnumerable<DeclarationParameter> _getDeclarationParameters ()
        {
            yield return new DeclarationParameter ("array_or_countable");
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 1)
            {
                throw new WrongParameterCountException (this, expected: 1, actual: parameters.Length);
            }

            FinalExpression param_1 = parameters [0].EvaluatedValue;

            if (param_1 is ArrayPointerExpression array_pointer)
            {
                ArrayItem item = array_pointer.Array.Shift ();
                return new Result (item?.Value ?? new NullExpression ());
            }
            else
            {
                return new Result (new NullExpression ());
            }
        }

    }
}
