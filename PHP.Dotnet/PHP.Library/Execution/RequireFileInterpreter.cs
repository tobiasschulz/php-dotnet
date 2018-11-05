using System;
using System.Collections.Immutable;
using System.Text;
using PHP.Tree;

namespace PHP.Execution
{
    public static class RequireFileInterpreter
    {
        public static Result Run (RequireFileExpression require, Scope scope)
        {
            string filepath_raw = Interpreters.Execute (require.FilePath, scope).ResultValue.GetStringValue ();

            scope.Root.Context.RequireFile (scope, IncludePathHelper.ResolveToRelative (filepath_raw, scope));

            return Result.NULL;
        }
    }


}
