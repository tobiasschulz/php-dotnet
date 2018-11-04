using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using PHP.Standard;
using PHP.Tree;
using System.Linq;

namespace PHP.Execution
{
    public static class RequireFileInterpreter
    {
        public static Result Run (RequireFileExpression require, Scope scope)
        {
            string basedir = null;
            scope.FindNearestScope<ScriptScope> (ss =>
            {
                basedir = ss.Script.GetScriptBaseDirectory ().Original.TrimEnd ('/', '\\');
            });

            string _stripBaseDir (string p)
            {
                if (basedir != null)
                {
                    p = p.ReplaceStart (basedir, "").Trim ('/', '\\');
                }
                return p;
            }


            string filepath_raw = Interpreters.Execute (require.FilePath, scope).ResultValue.GetStringValue ();

            List<NormalizedPath> possible_filepaths = new List<NormalizedPath> ();


            if (System.IO.Path.IsPathRooted (filepath_raw))
            {
                possible_filepaths.Add (_stripBaseDir (filepath_raw));
            }
            else
            {
                string scriptdir = null;
                scope.FindNearestScope<ScriptScope> (ss =>
                {
                    scriptdir = Path.GetDirectoryName (ss.Script.GetScriptPath ().Original);
                });

                if (scriptdir != null)
                {
                    scriptdir = _stripBaseDir (scriptdir);

                    Log.Debug ($"script in {scriptdir} includes {filepath_raw}");
                    List<string> scriptdir_parts = scriptdir.Split ('/').ToList ();
                    while (scriptdir_parts.Count != 0)
                    {
                        possible_filepaths.Add (Path.Combine (scriptdir_parts.Join ("/"), _stripBaseDir (filepath_raw)));
                        scriptdir_parts.RemoveAt (scriptdir_parts.Count - 1);
                    }
                }
                else
                {
                    possible_filepaths.Add (Path.Combine (scriptdir, _stripBaseDir (filepath_raw)));
                }
            }

            scope.Root.Context.RequireFile (possible_filepaths.ToArray ());

            return Result.NULL;
        }
    }


}
