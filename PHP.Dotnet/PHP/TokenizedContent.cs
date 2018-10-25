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
        public readonly ImmutableArray<Token> Tokens = new ImmutableArray<Token> ();

        public TokenizedContent (ImmutableArray<Token> tokens)
        {
            Tokens = tokens;
        }
    }
}
