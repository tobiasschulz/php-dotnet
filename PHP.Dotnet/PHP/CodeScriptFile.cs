using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;

namespace PHP
{
    public class CodeScriptFile : CodeScriptBase
    {
        public readonly string Path;
        public readonly string PathNormalized;

        public CodeScriptFile (Context context, string path)
            : base (context)
        {
            Path = path;
            PathNormalized = PathHelper.NormalizePath (path);
        }

        internal bool PathNormalizedEquals (string value)
        {
            return PathNormalized == value;
        }

        public override string ToString ()
        {
            return $"[CodeFile {Path}]";
        }

        protected override string RetrieveContent ()
        {
            return File.ReadAllText (Path);
        }
    }
}
