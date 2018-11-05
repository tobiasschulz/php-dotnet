using System.Collections.Generic;
using System.IO;
using PHP.Standard;
using PHP.Tree;
using System.Linq;
using System.Collections.Immutable;

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

        public static ImmutableArray<ScriptScope> GetAllScriptScopes (Scope scope)
        {
            List<ScriptScope> script_scopes = new List<ScriptScope> ();
            scope.ForAllScopes (s =>
            {
                if (s is IFunctionLikeScope fs && fs.GetDeclarationScopeOfFunction () is ScriptScope fss) script_scopes.Add (fss);
                if (s is ScriptScope ss) script_scopes.Add (ss);
            });
            return script_scopes.ToImmutableArray ();
        }

        public static NormalizedPath [] ResolveToRelative (string filepath_raw, Scope scope)
        {
            filepath_raw = filepath_raw.Replace (Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            ImmutableArray<ScriptScope> script_scopes = GetAllScriptScopes (scope);

            ImmutableArray<string> basedirs = script_scopes
                .Select (ss => ss.Script.GetScriptBaseDirectory ().Original.TrimEnd ('/', '\\'))
                .Distinct ()
                .ToImmutableArray ();

            ImmutableArray<string> scriptdirs = script_scopes
                .Select (ss => Path.GetDirectoryName (ss.Script.GetScriptPath ().Original))
                .Distinct ()
                .ToImmutableArray ();

            string _stripBaseDir (string p)
            {
                foreach (string basedir in basedirs)
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
                possible_filepaths.Add (_stripBaseDir (filepath_raw));

                Log.Debug ($"script in {scriptdirs.Join ("|")} includes {filepath_raw}");
                foreach (string scriptdir in scriptdirs)
                {
                    string scriptdir_without_basedir = _stripBaseDir (scriptdir);

                    List<string> scriptdir_parts = scriptdir_without_basedir.Split (Path.DirectorySeparatorChar).ToList ();
                    while (scriptdir_parts.Count != 0)
                    {
                        possible_filepaths.Add (Path.Combine (scriptdir_parts.Join (Path.DirectorySeparatorChar), _stripBaseDir (filepath_raw)));
                        scriptdir_parts.RemoveAt (scriptdir_parts.Count - 1);
                    }
                }
            }

            return possible_filepaths.ToArray ();
        }
    }

}
