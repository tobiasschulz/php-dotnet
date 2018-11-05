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
    public sealed class get_class : ManagedFunction
    {
        public get_class ()
            : base ("get_class")
        {
        }

        protected override IEnumerable<DeclarationParameter> _getDeclarationParameters ()
        {
            yield return new DeclarationParameter ("obj");
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 1)
            {
                throw new WrongParameterCountException (this, expected: 1, actual: parameters.Length);
            }

            FinalExpression param_1 = parameters [0].EvaluatedValue;

            if (param_1 is ObjectPointerExpression obj_pointer)
            {
                return new Result (new StringExpression (obj_pointer.Object.Classes.First ().Name.Value));
            }
            return new Result (new StringExpression (param_1.GetScalarAffinity ().ToString ()));
        }
    }
}
