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
    public sealed class defined : ManagedFunction
    {
        public defined ()
            : base ("defined")
        {
        }

        protected override IEnumerable<DeclarationParameter> _getDeclarationParameters ()
        {
            yield return new DeclarationParameter ("name");
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 1)
            {
                throw new WrongParameterCountException (this, expected: 1, actual: parameters.Length);
            }

            FinalExpression param_1 = parameters [0].EvaluatedValue;

            bool does_exist = function_scope.Root.Variables.Contains (param_1.GetStringValue ());

            return new Result (new BoolExpression (does_exist));
        }
    }
}
