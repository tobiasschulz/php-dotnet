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
    public sealed class in_array : ManagedFunction
    {
        public in_array ()
            : base ("in_array")
        {
        }

        protected override IEnumerable<DeclarationParameter> _getDeclarationParameters ()
        {
            yield return new DeclarationParameter ("needle");
            yield return new DeclarationParameter ("haystack");
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 2)
            {
                throw new WrongParameterCountException (this, expected: 2, actual: parameters.Length);
            }

            FinalExpression param_needle = parameters [0].EvaluatedValue;
            FinalExpression param_haystack = parameters [0].EvaluatedValue;

            if (param_haystack is ArrayPointerExpression array_pointer)
            {
                ArrayKey needle = param_needle.GetStringValue ();
                Log.Debug ($"check if element is in array: {needle}");
                return new Result (new BoolExpression (array_pointer.Array.GetAll ().Any (item =>
                {
                    return Interpreters.Execute (new BinaryExpression (param_needle, item.Value, BinaryOp.EQUAL), function_scope).ResultValue.GetBoolValue ();
                })));
            }
            else
            {
                return new Result (new BoolExpression (false));
            }
        }
    }
}
