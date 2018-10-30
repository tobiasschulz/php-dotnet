using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Standard;

namespace PHP
{
    public class CodeScriptEval : CodeScriptBase
    {
        private readonly string _code;
        public readonly NormalizedPath FullPath;
        public readonly NormalizedPath BaseDirectory;

        public CodeScriptEval (Context context, NormalizedPath base_directory, NormalizedPath relative_path, string code)
            : base (context)
        {
            _code = code;
            FullPath = new NormalizedPath (Path.Combine (base_directory.Original, relative_path.Original));
            BaseDirectory = base_directory;
        }


        public override string ToString ()
        {
            return $"[CodeEval {FullPath}]";
        }

        protected override string RetrieveContent ()
        {
            return _code;
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
