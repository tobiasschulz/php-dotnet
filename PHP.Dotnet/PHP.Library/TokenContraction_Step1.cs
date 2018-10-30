using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using System.Collections.Immutable;

namespace PHP
{
    internal class TokenContraction_Step1
    {
        private static ImmutableDictionary<char, char> _block_chars = new Dictionary<char, char>
        {
            ['('] = ')',
            ['['] = ']',
            ['{'] = '}',
        }.ToImmutableDictionary ();

        internal static RegularToken2 MakeTreeStep (IReadOnlyList<Token1> input)
        {
            RegularToken2 root = new RegularToken2 (TokenType2.BLOCK, string.Empty);
            RegularToken2 current = root;
            Stack<RegularToken2> previous_stack = new Stack<RegularToken2> ();

            for (int i = 0; i < input.Count; i++)
            {
                Token1 tok = input [i];

                switch (tok.Type)
                {
                    case TokenType1.COMMENT:
                        current.Add (new RegularToken2 (TokenType2.COMMENT, tok.Buffer));
                        break;
                    case TokenType1.OUTSIDE:
                        current.Add (new RegularToken2 (TokenType2.OUTSIDE, tok.Buffer));
                        break;
                    case TokenType1.IDENTIFIER:
                        current.Add (new RegularToken2 (TokenType2.IDENTIFIER, tok.Buffer));
                        break;
                    case TokenType1.STRING:
                        current.Add (new RegularToken2 (TokenType2.STRING, tok.Buffer));
                        break;
                    case TokenType1.VARIABLE:
                        current.Add (new RegularToken2 (TokenType2.VARIABLE, tok.Buffer));
                        break;
                    case TokenType1.CONTROL_CHAR:
                        if (_block_chars.TryGetValue (tok.Buffer [0], out char block_char_stop))
                        {
                            char block_char_start = tok.Buffer [0];
                            RegularToken2 child = new RegularToken2 (TokenType2.FLOW_CONTROL, block_char_start.ToString () + block_char_stop.ToString ());
                            current.Add (child);
                            previous_stack.Push (current);
                            current = child;
                        }
                        else if (_block_chars.ContainsValue (tok.Buffer [0]))
                        {
                            current = previous_stack.Pop ();
                        }
                        else
                        {
                            current.Add (new RegularToken2 (TokenType2.OPERATOR, tok.Buffer));
                        }
                        break;
                    case TokenType1.ARROW_ARRAY:
                        current.Add (new RegularToken2 (TokenType2.OPERATOR, "=>"));
                        break;
                    case TokenType1.ARROW_INSTANCE:
                        current.Add (new RegularToken2 (TokenType2.OPERATOR, "->"));
                        break;
                    case TokenType1.DOUBLE_COLON:
                        current.Add (new RegularToken2 (TokenType2.OPERATOR, "::"));
                        break;
                }
            }

            return root;
        }

    }
}
