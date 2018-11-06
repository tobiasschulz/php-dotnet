using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Standard;
using PHP.Library.Internal;
using PHP.Tree;
using System.Linq;

namespace PHP.Library.Functions
{
    public sealed class mkdir : ManagedFunction
    {
        public mkdir ()
            : base ("mkdir")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length > 3)
            {
                throw new WrongParameterCountException (this, expected: 1, actual: parameters.Length);
            }

            FinalExpression param_1 = parameters [0].EvaluatedValue;

            var possible_paths = IncludePathHelper.ResolveToFull (param_1.GetStringValue (), function_scope);

            Log.Debug ($"create directory: {possible_paths.Select (p => p.Original).Join (", ")}");

            return new Result (new BoolExpression (true));
        }
    }
}
