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
    public sealed class class_exists : ManagedFunction
    {
        public class_exists ()
            : base ("class_exists")
        {
        }

        protected override IEnumerable<DeclarationParameter> _getDeclarationParameters ()
        {
            yield return new DeclarationParameter ("name");
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
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
