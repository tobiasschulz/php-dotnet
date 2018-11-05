using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using PHP.Standard;
using PHP.Tree;
using PHP.Library.Internal;

namespace PHP
{
    public sealed class ContextOptions
    {
        public bool WRITE_AST;
        public bool DEBUG_EXECUTION;
    }

    public sealed class Context
    {
        public readonly ContextOptions Options;
        public readonly IParser Parser;
        public readonly RootScope RootScope;
        public readonly List<CodeDirectory> RootDirectories = new List<CodeDirectory> ();
        public readonly Dictionary<string, string> Defines = new Dictionary<string, string> ();
        public readonly IConsole Console = new StandardConsole ();
        public readonly HashSet<CodeScriptFile> IncludedFiles = new HashSet<CodeScriptFile> ();

        public Context (ContextOptions options, IParser parser)
        {
            Options = options ?? new ContextOptions ();
            Parser = parser;
            RootScope = new RootScope (this);
        }

        public void AddDirectory (string value)
        {
            RootDirectories.Add (new CodeDirectory (this, value));
        }

        public string Eval (string code, List<string> diagnostics = null, NormalizedPath base_directory = default, NormalizedPath relative_path = default)
        {
            base_directory = base_directory != default ? base_directory : NormalizedPath.DEFAULT_DOT;
            relative_path = relative_path != default ? relative_path : NormalizedPath.DEFAULT_DOT;

            CodeScriptEval file = new CodeScriptEval (this, base_directory, relative_path, code);

            return file.GetContent ().Eval (RootScope, Options, diagnostics);
        }

        public void RunFile (NormalizedPath value)
        {
            CodeScriptFile file = null;

            foreach (var d in RootDirectories)
            {
                file = d.Files.FirstOrDefault (f => f.FullPath == value);
                if (file != null) break;
            }

            if (file == null)
            {
                foreach (var d in RootDirectories)
                {
                    NormalizedPath relative_combined = Path.GetFullPath (Path.Combine (d.Path.Original, value.Original));
                    file = d.Files.FirstOrDefault (f => f.FullPath == relative_combined);
                    if (file != null) break;
                }
            }

            if (file == null)
            {
                throw new InterpreterException ($"File {value} could not be found. Root directories are: {RootDirectories.Join (", ")}");
            }

            if (IncludedFiles.Contains (file))
            {
                Log.Debug ($"File already included: {file}");
                return;
            }
            IncludedFiles.Add (file);

            file.GetContent ().Run (RootScope, Options);
        }

        public void RequireFile (Scope scope, params NormalizedPath [] possible_paths)
        {
            CodeScriptFile file = null;

            foreach (NormalizedPath possible_path in possible_paths)
            {
                if (file != null) break;

                foreach (var d in RootDirectories)
                {
                    file = d.Files.FirstOrDefault (f => f.FullPath == possible_path);
                    if (file != null) break;
                }

                if (file == null)
                {
                    foreach (var d in RootDirectories)
                    {
                        NormalizedPath relative_combined = Path.GetFullPath (Path.Combine (d.Path.Original, possible_path.Original));
                        file = d.Files.FirstOrDefault (f => f.FullPath == relative_combined);
                        if (file != null) break;
                    }
                }
            }

            if (file == null)
            {
                throw new InterpreterException ($"File {possible_paths.Join ("|")} could not be found. Root directories are: {RootDirectories.Join (", ")}.\n{scope.StackTrace}");
            }

            if (IncludedFiles.Contains (file))
            {
                Log.Debug ($"File already included: {file}");
                return;
            }
            IncludedFiles.Add (file);

            file.GetContent ().Run (scope, Options);
        }
    }
}
