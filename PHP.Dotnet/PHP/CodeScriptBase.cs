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
        private CodeContent _content;

        public CodeContent GetContent ()
        {
            if (_content == null)
            {
                _content = new CodeContent (RetrieveContent ());
            }
            return _content;
        }

        protected abstract string RetrieveContent ();

    }
}
