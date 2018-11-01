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
        public bool DEBUG_EXECUTION;
    }
    
    public sealed class Context
    {
        public readonly RootScope RootScope;
        public readonly ContextOptions Options;
        public readonly List<CodeDirectory> RootDirectories = new List<CodeDirectory> ();
        public readonly Dictionary<string, string> Defines = new Dictionary<string, string> ();
        public readonly IConsole Console = new StandardConsole ();

        public Context (ContextOptions options)
        {
            Options = options ?? new ContextOptions ();
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

            file.GetContent ().Run (RootScope, Options);
        }

        public void RequireFile (NormalizedPath value)
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

            file.GetContent ().Run (RootScope, Options);
        }
    }
}
