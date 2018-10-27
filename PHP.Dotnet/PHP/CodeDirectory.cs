using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Immutable;
using PHP.Helper;
using PHP.Standard;

namespace PHP
{
    public sealed class CodeDirectory
    {
        private readonly Context _context;
        public readonly string Path;
        public readonly ImmutableArray<CodeScriptFile> Files = ImmutableArray<CodeScriptFile>.Empty;

        public CodeDirectory (Context context, string path)
        {
            _context = context;
            Path = path;

            if (Directory.Exists (path))
            {
                Files = Directory.GetFiles (path, "*.php", SearchOption.AllDirectories)
                    .Select (p => new CodeScriptFile (context, p))
                    .ToImmutableArray ();
            }
        }

        public override string ToString ()
        {
            return $"[CodeDirectory {Path}]";
        }
    }
}
