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
        public readonly NormalizedPath FullPath;
        public readonly NormalizedPath RelativePath;
        public readonly NormalizedPath BaseDirectory;

        public CodeScriptFile (Context context, CodeDirectory base_directory, string full_path)
            : base (context)
        {
            FullPath = new NormalizedPath (full_path);
            BaseDirectory = base_directory.Path;
        }

        public override string ToString ()
        {
            return $"[CodeFile {FullPath}]";
        }

        protected override string RetrieveContent ()
        {
            return File.ReadAllText (FullPath.Original);
        }

        public override NormalizedPath GetScriptPath ()
        {
            return FullPath;
        }

        public override NormalizedPath GetScriptBaseDirectory ()
        {
            return BaseDirectory;
        }
    }
}
