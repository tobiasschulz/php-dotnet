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
    public sealed class array_key_exists : ManagedFunction
    {
        public array_key_exists ()
            : base ("array_key_exists")
        {
        }

        protected override IEnumerable<DeclarationParameter> _getDeclarationParameters ()
        {
            yield return new DeclarationParameter ("key");
            yield return new DeclarationParameter ("array");
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 2)
            {
                throw new WrongParameterCountException (this, expected: 2, actual: parameters.Length);
            }

            FinalExpression param_key = parameters [0].EvaluatedValue;
            FinalExpression param_array = parameters [0].EvaluatedValue;

            if (param_array is ArrayPointerExpression array_pointer)
            {
                ArrayKey key = param_key.GetStringValue ();
                Log.Debug ($"check if array key exists: {key}");
                return new Result (new BoolExpression (array_pointer.Array.Contains (key)));
            }
            else
            {
                return new Result (new BoolExpression (false));
            }
        }
    }
}
