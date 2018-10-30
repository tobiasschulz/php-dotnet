using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using PHP.Helper;
using PHP.Standard;
using PHP.Tree;
using PHP.Library.Internal;

namespace PHP
{
    public sealed class Context
    {
        public readonly RootScope RootScope;
        public readonly List<CodeDirectory> RootDirectories = new List<CodeDirectory> ();
        public readonly Dictionary<string, string> Defines = new Dictionary<string, string> ();
        public readonly IConsole Console = new StandardConsole ();

        public Context ()
        {
            RootScope = new RootScope (this);
        }

        public void AddDirectory (string value)
        {
            RootDirectories.Add (new CodeDirectory (this, value));
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

            file.GetContent ().Run (RootScope);
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

            file.GetContent ().Run (RootScope);
        }
    }
}
