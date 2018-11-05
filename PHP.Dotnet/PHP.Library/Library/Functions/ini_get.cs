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
    public sealed class ini_get : ManagedFunction
    {
        public ini_get ()
            : base ("ini_get")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 1)
            {
                throw new WrongParameterCountException (this, expected: 1, actual: parameters.Length);
            }

            FinalExpression param_1 = parameters [0].EvaluatedValue;

            IVariable variable = function_scope.Root.Variables.EnsureExists (new NameOfVariable (param_1.GetStringValue ()));
            return new Result (variable.Value);
        }
    }
}
