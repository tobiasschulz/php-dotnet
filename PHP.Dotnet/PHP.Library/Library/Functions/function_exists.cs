using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class function_exists : Function
    {
        public function_exists ()
            : base ("function_exists")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedCallParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 1)
            {
                throw new WrongParameterCountException (this, expected: 1, actual: parameters.Length);
            }

            FinalExpression param_1 = parameters [0].EvaluatedValue;

            bool does_exist = function_scope.Root.GlobalFunctions.Contains (new Name (param_1.GetStringValue ()));

            return new Result (new BoolExpression (does_exist));
        }
    }
}
