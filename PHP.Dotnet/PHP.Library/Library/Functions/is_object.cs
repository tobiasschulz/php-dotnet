using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class is_object : ManagedFunction
    {
        public is_object ()
            : base ("is_object")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 1)
            {
                throw new WrongParameterCountException (this, expected: 1, actual: parameters.Length);
            }

            FinalExpression param_1 = parameters [0].EvaluatedValue;

            if (param_1.GetScalarAffinity () == ScalarAffinity.OBJECT)
            {
                return new Result (new BoolExpression (true));
            }
            else
            {
                return new Result (new BoolExpression (false));
            }
        }
    }
}
