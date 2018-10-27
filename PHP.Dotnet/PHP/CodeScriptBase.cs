using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;

namespace PHP
{
    public abstract class CodeScriptBase
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
                _content = new CodeContent (_context, RetrieveContent ());
            }
            return _content;
        }

        protected abstract string RetrieveContent ();

    }
}
