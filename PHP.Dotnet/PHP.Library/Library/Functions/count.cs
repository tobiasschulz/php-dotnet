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
    public sealed class count : _base_count
    {
        public count ()
            : base ("count")
        {
        }
    }

    public sealed class @sizeof : _base_count
    {
        public @sizeof ()
            : base ("sizeof")
        {
        }
    }

    public abstract class _base_count : ManagedFunction
    {
        public _base_count (string name)
            : base (name)
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
                return new Result (new LongExpression (array_pointer.Array.GetAll ().Count ()));
            }
            else
            {
                return new Result (new LongExpression (0));
            }
        }
    }
}
