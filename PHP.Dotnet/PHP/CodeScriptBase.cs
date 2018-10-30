using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using PHP.Tree;

namespace PHP
{
    public abstract class CodeScriptBase : IScript
    {
        protected readonly Context _context;
        private CodeContent _content;

        public CodeScriptBase (Context context)
        {
            _context = context;
        }

        public CodeContent GetContent ()
        {
            if (_content == null)
            {
                _content = new CodeContent (_context, RetrieveContent (), this);
            }
            return _content;
        }

        protected abstract string RetrieveContent ();
        public abstract NormalizedPath GetScriptPath ();
        public abstract NormalizedPath GetScriptBaseDirectory ();

    }
}
