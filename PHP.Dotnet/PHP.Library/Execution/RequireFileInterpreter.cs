using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class RequireFileInterpreter
    {
        public static Result Run (RequireFileExpression require, Scope scope)
        {
            string filepath = Interpreters.Execute (require.FilePath, scope).ResultValue.GetStringValue ();

            if (!System.IO.Path.IsPathRooted (filepath))
            {
                string scriptdir = null;
                scope.FindNearestScope<ScriptScope> (ss =>
                {
                    scriptdir = Path.GetDirectoryName (ss.Script.GetScriptPath ().Original);
                });

                filepath = Path.Combine (scriptdir, filepath);
            }

            string basedir = null;
            scope.FindNearestScope<ScriptScope> (ss =>
            {
                basedir = ss.Script.GetScriptBaseDirectory ().Original.TrimEnd ('/', '\\');
            });

            if (basedir != null)
            {
                filepath = filepath.ReplaceStart (basedir, "").Trim ('/', '\\');
            }


            scope.Root.Context.RequireFile (filepath);

            return Result.NULL;
        }
    }


}
