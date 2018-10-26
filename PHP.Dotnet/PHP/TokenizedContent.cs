using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using System.Collections.Immutable;

namespace PHP
{
    internal class TokenizedContent
    {
        public readonly ImmutableArray<BaseToken2> Tokens = new ImmutableArray<BaseToken2> ();

        public TokenizedContent (params BaseToken2 [] tokens)
        {
            Tokens = tokens.ToImmutableArray ();
        }

        internal void DumpLog ()
        {
            foreach (BaseToken2 tok in Tokens)
            {
                tok.DumpLog (indent: 0);
            }
        }
    }
}
