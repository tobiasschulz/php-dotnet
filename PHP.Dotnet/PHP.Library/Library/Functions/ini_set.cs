using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Library.TypeSystem;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class ini_set : Function
    {
        public ini_set ()
            : base ("ini_set")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedCallParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 2)
            {
                throw new WrongParameterCountException (this, expected: 2, actual: parameters.Length);
            }

            FinalExpression param_1 = parameters [0].EvaluatedValue;
            FinalExpression param_2 = parameters [1].EvaluatedValue;

            function_scope.Root.Variables.EnsureExists (new VariableName (param_1.GetStringValue ()), out IVariable variable);
            variable.Value = param_2;

            return Result.NULL;
        }
    }
}
