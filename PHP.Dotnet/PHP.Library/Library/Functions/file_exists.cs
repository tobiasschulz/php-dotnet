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
    public sealed class file_exists : ManagedFunction
    {
        public file_exists ()
            : base ("file_exists")
        {
        }

        protected override Result _execute (ImmutableArray<EvaluatedCallParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 1)
            {
                throw new WrongParameterCountException (this, expected: 1, actual: parameters.Length);
            }

            FinalExpression param_1 = parameters [0].EvaluatedValue;

            var possible_paths = IncludePathHelper.ResolveToFull (param_1.GetStringValue (), function_scope);

            Log.Debug ($"check if file exists: {possible_paths.Select (p => p.Original).Join (", ")}");
            bool does_exist = possible_paths.Any (p => System.IO.File.Exists (p.Original));

            return new Result (new BoolExpression (does_exist));
        }
    }
}
