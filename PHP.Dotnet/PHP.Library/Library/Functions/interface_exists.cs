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
    public sealed class interface_exists : ManagedFunction
    {
        public interface_exists ()
            : base ("interface_exists")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedCallParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 1 && parameters.Length != 2)
            {
                throw new WrongParameterCountException (this, expected: 1, actual: parameters.Length);
            }

            FinalExpression param_1 = parameters [0].EvaluatedValue;

            bool does_exist = function_scope.Root.Classes.Contains (param_1.GetStringValue ());

            return new Result (new BoolExpression (does_exist));
        }
    }
}
