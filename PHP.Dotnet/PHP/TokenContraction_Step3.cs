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
    internal class TokenContraction_Step3
    {
        internal static BaseToken2 Transform (RegularToken2 input)
        {
            return _transform (input);
        }

        private static BaseToken2 _transform (BaseToken2 input)
        {
            return _transform (new [] { input }).Single ();
        }

        private static IReadOnlyList<BaseToken2> _transform (IReadOnlyList<BaseToken2> input)
        {
            return _makeFunctions (_makeFlowControls (_transformInstanceAccess (input)));
        }

        private static IReadOnlyList<BaseToken2> _makeFlowControls (IReadOnlyList<BaseToken2> input)
        {
            List<BaseToken2> output = new List<BaseToken2> ();

            for (int i = 0; i < input.Count; i++)
            {
                BaseToken2 tok = input [i];
                if (tok is RegularToken2 tokr)
                {
                    if (i + 1 < input.Count && input [i + 1] is RegularToken2 tok_next)
                    {
                        if (tokr.Buffer.IsAny ("try", "else", "catch", "finally") && tok_next.Type == TokenType2.FLOW_CONTROL && tok_next.Buffer == "{}")
                        {
                            FlowControlToken2 tok_fixed = new FlowControlToken2 (
                                name: tokr.Buffer,
                                condition: null,
                                block: _makeBlock(Transform (tok_next))
                            );
                            output.Add (tok_fixed);
                            i += 1; // one extra
                            continue;
                        }

                        if (i + 2 < input.Count && input [i + 2] is RegularToken2 tok_next2)
                        {
                            if (tokr.Buffer.IsAny ("if", "elseif", "while", "for", "catch") && tok_next.Type == TokenType2.FLOW_CONTROL && tok_next.Buffer == "()" && tok_next2.Type == TokenType2.FLOW_CONTROL && tok_next2.Buffer == "{}")
                            {
                                FlowControlToken2 tok_fixed = new FlowControlToken2 (
                                    name: tokr.Buffer,
                                    condition: _makeBlock ( Transform (tok_next)),
                                    block: _makeBlock( Transform (tok_next2))
                                );
                                output.Add (tok_fixed);
                                i += 2; // two extra
                                continue;
                            }

                            if (i + 3 < input.Count && input [i + 3] is RegularToken2 tok_next3)
                            {
                                if (tokr.Buffer.IsAny ("do") && tok_next.Type == TokenType2.FLOW_CONTROL && tok_next.Buffer == "{}" && tok_next2.Buffer == "while" && tok_next3.Type == TokenType2.FLOW_CONTROL && tok_next3.Buffer == "()")
                                {
                                    FlowControlToken2 tok_fixed = new FlowControlToken2 (
                                        name: tokr.Buffer,
                                        condition: _makeBlock (Transform (tok_next3)),
                                        block: _makeBlock(Transform (tok_next))
                                    );
                                    output.Add (tok_fixed);
                                    i += 3; // three extra
                                    continue;
                                }
                            }
                        }
                    }

                    output.Add (new RegularToken2 (tokr.Type, tokr.Buffer, _transform (tokr.Children)));
                }
                else
                {
                    output.Add (tok);
                }
            }

            return output;
        }

        private static BaseToken2 _makeBlock (BaseToken2 tok)
        {
            if (tok is RegularToken2 tokr && tokr.Type == TokenType2.FLOW_CONTROL)
            {
                return new RegularToken2 (TokenType2.BLOCK, string.Empty, tokr.Children);
            }
            return tok;
        }

        private static IReadOnlyList<BaseToken2> _makeFunctions (IReadOnlyList<BaseToken2> input)
        {
            List<BaseToken2> output = new List<BaseToken2> ();

            for (int i = 0; i < input.Count; i++)
            {
                BaseToken2 tok = input [i];

                if (i + 1 < input.Count && input [i + 1] is RegularToken2 tok_next && tok_next.Type == TokenType2.FLOW_CONTROL && tok_next.Buffer == "()")
                {
                    FunctionToken2 tok_fixed = new FunctionToken2 (
                        function_ref: tok,
                        caller: null,
                        arguments: Transform (tok_next)
                    );
                    output.Add (tok_fixed);
                    i += 1; // one extra
                    continue;
                }

                if (tok is RegularToken2 tokr)
                {
                    output.Add (new RegularToken2 (tokr.Type, tokr.Buffer, _transform (tokr.Children)));
                }
                else
                {
                    output.Add (tok);
                }
            }

            return output;
        }

        private static IReadOnlyList<BaseToken2> _transformInstanceAccess (IReadOnlyList<BaseToken2> input)
        {
            List<BaseToken2> list = input.ToList ();

            for (int i = 0; i < list.Count;)
            {
                BaseToken2 tok = list [i];
                if (i + 2 < list.Count && list [i + 1] is RegularToken2 tok_next && tok_next.Buffer.IsAny ("->", "::") && list [i + 2] is BaseToken2 tok_next2)
                {
                    bool is_func_access = false;
                    if (i + 3 < input.Count && input [i + 3] is RegularToken2 tok_next3 && tok_next3.Type == TokenType2.FLOW_CONTROL && tok_next3.Buffer == "()")
                    {
                        is_func_access = true;
                    }

                    if (!is_func_access)
                    {
                        ClassAccessToken2 tok_fixed = new ClassAccessToken2 (
                            kind: tok_next.Buffer,
                            caller: _transform (tok),
                            property: _transform (tok_next2)
                        );

                        list.RemoveAt (i + 2);
                        list.RemoveAt (i + 1);
                        list [i] = tok_fixed;

                        continue;
                    }
                }

                i++;
            }

            return list;
        }
    }
}
