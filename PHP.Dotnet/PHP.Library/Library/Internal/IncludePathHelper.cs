using System.Collections.Generic;
using System.IO;
using PHP.Standard;
using PHP.Tree;
using System.Linq;

namespace PHP.Execution
{
    public static class IncludePathHelper
    {
        public static NormalizedPath [] ResolveToFull (string filepath_raw, Scope scope)
        {
            IEnumerable<NormalizedPath> _expand (NormalizedPath [] relative_paths)
            {
                foreach (NormalizedPath relative_path in relative_paths)
                {
                    foreach (var d in scope.Root.Context.RootDirectories)
                    {
                        NormalizedPath full_path = Path.GetFullPath (Path.Combine (d.Path.Original, relative_path.Original));
                        yield return full_path;
                    }
                }
            }
            return _expand (ResolveToRelative (filepath_raw, scope)).ToArray ();
        }

        public static ScriptScope GetBestScriptScope (Scope scope)
        {
            ScriptScope script_scope = null;
            if (script_scope == null)
                scope.FindNearestScope<IFunctionLikeScope> (fs => script_scope = fs.GetDeclarationScopeOfFunction ());
            if (script_scope == null)
                scope.FindNearestScope<ScriptScope> (ss => script_scope = ss);
            return script_scope;
        }

        public static NormalizedPath [] ResolveToRelative (string filepath_raw, Scope scope)
        {
            filepath_raw = filepath_raw.Replace (Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            ScriptScope script_scope = GetBestScriptScope (scope);

            string basedir = null;
            string scriptdir = null;
            if (script_scope != null)
            {
                basedir = script_scope.Script.GetScriptBaseDirectory ().Original.TrimEnd ('/', '\\');
                scriptdir = Path.GetDirectoryName (script_scope.Script.GetScriptPath ().Original);
            }

            string _stripBaseDir (string p)
            {
                if (basedir != null)
                {
                    p = p.ReplaceStart (basedir, "").Trim ('/', '\\');
                }
                return p;
            }

            List<NormalizedPath> possible_filepaths = new List<NormalizedPath> ();

            if (System.IO.Path.IsPathRooted (filepath_raw))
            {
                possible_filepaths.Add (_stripBaseDir (filepath_raw));
            }
            else
            {
                if (scriptdir != null)
                {
                    scriptdir = _stripBaseDir (scriptdir);

                    Log.Debug ($"script in {scriptdir} includes {filepath_raw}");
                    List<string> scriptdir_parts = scriptdir.Split (Path.DirectorySeparatorChar).ToList ();
                    while (scriptdir_parts.Count != 0)
                    {
                        possible_filepaths.Add (Path.Combine (scriptdir_parts.Join (Path.DirectorySeparatorChar), _stripBaseDir (filepath_raw)));
                        scriptdir_parts.RemoveAt (scriptdir_parts.Count - 1);
                    }
                }
                else
                {
                    possible_filepaths.Add (Path.Combine (scriptdir, _stripBaseDir (filepath_raw)));
                }
            }

            return possible_filepaths.ToArray ();
        }
    }

}
