﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using PHP.Helper;
using PHP.Standard;
using PHP.Tree;

namespace PHP
{
    public sealed class Context
    {
        public readonly List<CodeDirectory> RootDirectories = new List<CodeDirectory> ();
        public readonly Dictionary<string, string> Defines = new Dictionary<string, string> ();

        public readonly GlobalScope GlobalScope = new GlobalScope ();

        public void AddDirectory (string value)
        {
            RootDirectories.Add (new CodeDirectory (this, value));
        }

        public void RunFile (string value)
        {
            string value_normalized = PathHelper.NormalizePath (value);

            CodeScriptFile file = RootDirectories.SelectMany (d => d.Files).FirstOrDefault (f => f.PathNormalizedEquals (value_normalized));
            if (file == null)
            {
                throw new InterpreterException ($"File {value} could not be found. Root directories are: {RootDirectories.Join (", ")}");
            }

            file.GetContent ().Run (GlobalScope);
        }
    }
}
