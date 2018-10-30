using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using System.Collections.Immutable;

namespace PHP
{
    public enum TokenType1
    {
        NONE = 0,
        OUTSIDE = 1,
        STRING = 2,
        IDENTIFIER = 3,
        CONTROL_CHAR = 4,
        COMMENT = 5,
        ARROW_ARRAY = 6,
        SPACE = 7,
        ARROW_INSTANCE = 8,
        DOUBLE_COLON = 9,
        VARIABLE = 10,
    }

    public sealed class Token1
    {
        public readonly TokenType1 Type;
        public readonly string Buffer;

        public Token1 (TokenType1 type, string buffer)
        {
            Type = type;
            Buffer = buffer;
        }

        public override string ToString ()
        {
            return $"[Token: {Type} '{Buffer}']";
        }
    }

}
