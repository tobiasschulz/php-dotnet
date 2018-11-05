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
    public sealed class define : ManagedFunction
    {
        public define ()
            : base ("define")
        {
        }

        protected override IEnumerable<DeclarationParameter> _getDeclarationParameters ()
        {
            yield return new DeclarationParameter ("name");
            yield return new DeclarationParameter ("value");
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 2)
            {
                throw new WrongParameterCountException (this, expected: 2, actual: parameters.Length);
            }

            FinalExpression param_1 = parameters [0].EvaluatedValue;
            FinalExpression param_2 = parameters [1].EvaluatedValue;

            IVariable variable = function_scope.Root.Variables.EnsureExists (new NameOfVariable (param_1.GetStringValue ()));
            variable.Value = param_2;

            return Result.NULL;
        }
    }
}
