using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using System.Collections.Immutable;

namespace PHP
{
    internal class SyntaxTree
    {
        public readonly ImmutableArray<SyntaxTreeElement> Elements;

        public SyntaxTree (ImmutableArray<SyntaxTreeElement> elements)
        {
            Elements = elements;
        }

        internal void DumpLog ()
        {
            foreach (SyntaxTreeElement element in Elements)
            {
                element.DumpLog (indent: 0);
            }
        }
    }

    public sealed class SyntaxTreeStatement : SyntaxTreeElement
    {
        public readonly ImmutableArray<SyntaxTreeElement> Elements;

        public SyntaxTreeStatement (ImmutableArray<SyntaxTreeElement> elements)
        {
            Elements = elements;
        }

        internal static bool TryDetect (ReadOnlySpan<Token1> tokens, ref int offset, out SyntaxTreeStatement result)
        {
            result = null;
            for (int i = offset; i < tokens.Length; i++)
            {
                Token1 token = tokens [i];
                if (token.Type == TokenType1.CONTROL_CHAR && token.Buffer == ";")
                {
                    result = new SyntaxTreeStatement (
                        SyntaxTreeAnalyzer.Analyze<SyntaxTreeStatement> (
                            tokens.Slice (offset, i - offset),
                            new [] { typeof (SyntaxTreeStatement) }
                        )
                    );
                    offset = i + 1;
                    return true;
                }
            }
            return false;
        }

        internal override void DumpLog (int indent)
        {
            _log (indent, $"{this.GetType ().Name}:");
            foreach (var e in Elements)
            {
                e.DumpLog (indent + 1);
            }
        }
    }

    public sealed class SyntaxTreeLeftValue : SyntaxTreeElement
    {
        public readonly ImmutableArray<SyntaxTreeElement> Elements;

        public SyntaxTreeLeftValue (ImmutableArray<SyntaxTreeElement> elements)
        {
            Elements = elements;
        }

        internal static bool TryDetect (ReadOnlySpan<Token1> tokens, ref int offset, out SyntaxTreeLeftValue result)
        {
            result = null;
            int i = offset;
            _skipTokens (tokens, ref i, TokenType1.SPACE, TokenType1.OUTSIDE, TokenType1.COMMENT);
            if (i < tokens.Length)
            {
                Token1 token = tokens [i];
                if (token.Type.IsAny (TokenType1.VARIABLE, TokenType1.IDENTIFIER))
                {
                    i++;
                    _skipTokens (tokens, ref i, TokenType1.SPACE, TokenType1.OUTSIDE, TokenType1.COMMENT, TokenType1.VARIABLE);
                    while (i + 1 < tokens.Length && tokens [i].Type.IsAny (TokenType1.ARROW_INSTANCE, TokenType1.DOUBLE_COLON))
                    {
                        if (token.Type.IsAny (TokenType1.VARIABLE, TokenType1.IDENTIFIER))
                        {
                            i += 2;
                            _skipTokens (tokens, ref i, TokenType1.SPACE, TokenType1.OUTSIDE, TokenType1.COMMENT, TokenType1.VARIABLE);
                        }
                    }

                    result = new SyntaxTreeLeftValue (
                        SyntaxTreeAnalyzer.Analyze<SyntaxTreeLeftValue> (
                            tokens.Slice (offset, i - offset),
                            new [] { typeof (SyntaxTreeLeftValue), typeof (SyntaxTreeAssignment), typeof (SyntaxTreeCall) }
                        )
                    );
                    offset = i;
                    return true;
                }
            }
            return false;
        }

        internal override void DumpLog (int indent)
        {
            _log (indent, $"{this.GetType ().Name}:");
            foreach (var e in Elements)
            {
                e.DumpLog (indent + 1);
            }
        }
    }

    public sealed class SyntaxTreeAssignment : SyntaxTreeElement
    {
        public readonly SyntaxTreeLeftValue Left;
        public readonly ImmutableArray<SyntaxTreeElement> ElementsRight;

        public SyntaxTreeAssignment (SyntaxTreeLeftValue left, ImmutableArray<SyntaxTreeElement> elements_right)
        {
            Left = left;
            ElementsRight = elements_right;
        }

        internal static bool TryDetect (ReadOnlySpan<Token1> tokens, ref int offset, out SyntaxTreeAssignment result)
        {
            result = null;
            int i = offset;
            if (SyntaxTreeLeftValue.TryDetect (tokens, ref i, out var result_leftvalue) && i < tokens.Length)
            {
                Token1 token = tokens [i];
                Log.Debug ($"ass: {token}");
                if (token.Type == TokenType1.CONTROL_CHAR && token.Buffer == "=")
                {
                    i++;
                    result = new SyntaxTreeAssignment (
                        result_leftvalue,
                        SyntaxTreeAnalyzer.Analyze<SyntaxTreeAssignment> (
                            tokens.Slice (i),
                            new [] { typeof (SyntaxTreeAssignment) }
                        )
                    );
                    offset = tokens.Length;
                    return true;
                }
            }
            return false;
        }

        internal override void DumpLog (int indent)
        {
            _log (indent, $"{this.GetType ().Name}:");
            _log (indent, $"  left:");
            Left.DumpLog (indent + 1);
            _log (indent, $"  right:");
            foreach (var e in ElementsRight)
            {
                e.DumpLog (indent + 1);
            }
        }
    }

    public sealed class SyntaxTreeTry : SyntaxTreeControlFlow
    {
        public SyntaxTreeTry (ImmutableArray<SyntaxTreeElement> elements)
            : base (elements)
        {
        }

        internal static bool TryDetect (ReadOnlySpan<Token1> tokens, ref int offset, out SyntaxTreeTry result)
        {
            result = null;
            int i = offset;
            _skipTokens (tokens, ref i, TokenType1.SPACE, TokenType1.OUTSIDE, TokenType1.COMMENT);
            if (i + 1 < tokens.Length)
            {
                Token1 token = tokens [i];
                if (token.Type == TokenType1.IDENTIFIER && token.Buffer == "try")
                {
                    i++;
                    if (_detectBlock (tokens, "{", "}", i, out int block_index_start, out int block_index_stop))
                    {
                        i = block_index_stop;
                        result = new SyntaxTreeTry (
                            SyntaxTreeAnalyzer.Analyze<SyntaxTreeStatement> (
                                tokens.Slice (block_index_start, i - block_index_start),
                                new [] { typeof (SyntaxTreeStatement) }
                            )
                        );
                        offset = i + 1;
                        return true;
                    }
                }
            }
            return false;
        }

    }

    public sealed class SyntaxTreeArray : SyntaxTreeControlFlow
    {
        public SyntaxTreeArray (ImmutableArray<SyntaxTreeElement> elements)
            : base (elements)
        {
        }

        internal static bool TryDetect (ReadOnlySpan<Token1> tokens, ref int offset, out SyntaxTreeArray result)
        {
            result = null;
            int i = offset;
            _skipTokens (tokens, ref i, TokenType1.SPACE, TokenType1.OUTSIDE, TokenType1.COMMENT);
            if (i + 1 < tokens.Length)
            {
                Token1 token = tokens [i];
                if (token.Type == TokenType1.IDENTIFIER && token.Buffer == "array")
                {
                    i++;
                    token = tokens [i];
                    Log.Debug ($"array: {token}");
                    if (_detectBlock (tokens, "(", ")", i, out int block_index_start, out int block_index_stop))
                    {
                        i = block_index_stop;
                        result = new SyntaxTreeArray (
                            SyntaxTreeAnalyzer.Analyze<SyntaxTreeArray> (
                                tokens.Slice (block_index_start, i - block_index_start),
                                new [] { typeof (SyntaxTreeArray) },
                                allow_array_entry: true
                            )
                        );
                        offset = i + 1;
                        return true;
                    }
                }
                else if (_detectBlock (tokens, "[", "]", i, out int block_index_start, out int block_index_stop))
                {
                    i = block_index_stop;
                    result = new SyntaxTreeArray (
                        SyntaxTreeAnalyzer.Analyze<SyntaxTreeArray> (
                            tokens.Slice (block_index_start, i - block_index_start),
                            new [] { typeof (SyntaxTreeArray) },
                            allow_array_entry: true
                        )
                    );
                    offset = i + 1;
                    return true;
                }
            }
            return false;
        }

    }

    public sealed class SyntaxTreeArrayKey : SyntaxTreeElement
    {
        public readonly ImmutableArray<SyntaxTreeElement> Elements;

        public SyntaxTreeArrayKey (ImmutableArray<SyntaxTreeElement> elements)
        {
            Elements = elements;
        }

        internal static bool TryDetect (ReadOnlySpan<Token1> tokens, ref int offset, out SyntaxTreeArrayKey result)
        {
            result = null;
            int i = offset;
            _skipTokens (tokens, ref i, TokenType1.SPACE, TokenType1.OUTSIDE, TokenType1.COMMENT);
            if (i < tokens.Length)
            {
                Token1 token = tokens [i];
                Log.Debug ($"array entry test: {token}");
                if (token.Type.IsAny (TokenType1.IDENTIFIER, TokenType1.STRING))
                {
                    i++;
                    _skipTokens (tokens, ref i, TokenType1.SPACE, TokenType1.OUTSIDE, TokenType1.COMMENT);

                    result = new SyntaxTreeArrayKey (
                        SyntaxTreeAnalyzer.Analyze<SyntaxTreeArrayKey> (
                            tokens.Slice (offset, i - offset),
                            all_complex_skipped: true
                        )
                    );
                    offset = i;
                    return true;
                }
            }
            return false;
        }

        internal override void DumpLog (int indent)
        {
            _log (indent, $"{this.GetType ().Name}:");
            foreach (var e in Elements)
            {
                e.DumpLog (indent + 1);
            }
        }
    }

    public sealed class SyntaxTreeArrayEntry : SyntaxTreeElement
    {
        public readonly SyntaxTreeArrayKey Key;
        public readonly ImmutableArray<SyntaxTreeElement> Value;

        public SyntaxTreeArrayEntry (SyntaxTreeArrayKey key, ImmutableArray<SyntaxTreeElement> value)
        {
            Key = key;
            Value = value;
        }

        internal static bool TryDetect (ReadOnlySpan<Token1> tokens, ref int offset, out SyntaxTreeArrayEntry result)
        {
            result = null;
            int i = offset;
            if (SyntaxTreeArrayKey.TryDetect (tokens, ref i, out var result_array_key) && i < tokens.Length)
            {
                Token1 token = tokens [i];
                Log.Debug ($"array entry: {token}");
                if (token.Type == TokenType1.ARROW_ARRAY)
                {
                    i++;
                }
            }

            _skipTokens (tokens, ref i, TokenType1.SPACE, TokenType1.OUTSIDE, TokenType1.COMMENT);
            int index_start_value = i;
            int index_stop_value = tokens.Length - 1;

            Log.Debug ($"array entry start value: {tokens[index_start_value]}");

            for (; i < tokens.Length; i++)
            {
                if (tokens [i].Type == TokenType1.CONTROL_CHAR && tokens [i].Buffer == ",")
                {
                    index_stop_value = i;
                    break;
                }
            }

            Log.Debug ($"array entry stop value: {tokens [index_stop_value]}");

            result = new SyntaxTreeArrayEntry (
                  result_array_key,
                  SyntaxTreeAnalyzer.Analyze<SyntaxTreeArrayEntry> (
                      tokens.Slice (index_start_value, index_stop_value - index_start_value),
                      new [] { typeof (SyntaxTreeArrayEntry) }
                  )
              );
            offset = index_stop_value + 1;
            return true;
        }

        internal override void DumpLog (int indent)
        {
            _log (indent, $"{this.GetType ().Name}:");
            _log (indent, $"  key:");
            Key.DumpLog (indent + 1);
            _log (indent, $"  value:");
            foreach (var e in Value)
            {
                e.DumpLog (indent + 1);
            }
        }
    }

    public sealed class SyntaxTreeCall : SyntaxTreeElement
    {
        public readonly SyntaxTreeLeftValue Left;
        public readonly ImmutableArray<SyntaxTreeElement> ElementsRight;

        public SyntaxTreeCall (SyntaxTreeLeftValue left, ImmutableArray<SyntaxTreeElement> elements_right)
        {
            Left = left;
            ElementsRight = elements_right;
        }

        internal static bool TryDetect (ReadOnlySpan<Token1> tokens, ref int offset, out SyntaxTreeCall result)
        {
            result = null;
            int i = offset;
            if (SyntaxTreeLeftValue.TryDetect (tokens, ref i, out var result_leftvalue) && i < tokens.Length)
            {
                Token1 token = tokens [i];
                Log.Debug ($"call: {token}");
                if (_detectBlock (tokens, "(", ")", i, out int block_index_start, out int block_index_stop))
                {
                    i = block_index_stop;
                    result = new SyntaxTreeCall (
                        result_leftvalue,
                        SyntaxTreeAnalyzer.Analyze<SyntaxTreeCall> (
                            tokens.Slice (block_index_start, i - block_index_start),
                            new [] { typeof (SyntaxTreeCall) }
                        )
                    );
                    offset = i + 1;
                    return true;
                }
            }
            return false;
        }

        internal override void DumpLog (int indent)
        {
            _log (indent, $"{this.GetType ().Name}:");
            _log (indent, $"  function:");
            Left.DumpLog (indent + 1);
            _log (indent, $"  arguments:");
            foreach (var e in ElementsRight)
            {
                e.DumpLog (indent + 1);
            }
        }
    }

    public sealed class SyntaxTreeIf : SyntaxTreeControlFlow
    {
        public SyntaxTreeIf (ImmutableArray<SyntaxTreeElement> elements)
            : base (elements)
        {
        }
    }

    public abstract class SyntaxTreeControlFlow : SyntaxTreeElement
    {
        public readonly ImmutableArray<SyntaxTreeElement> ElementsInBraces;

        public SyntaxTreeControlFlow (ImmutableArray<SyntaxTreeElement> elements_in_braces)
        {
            ElementsInBraces = elements_in_braces;
        }

        internal override void DumpLog (int indent)
        {
            _log (indent, $"{this.GetType ().Name}:");
            foreach (var e in ElementsInBraces)
            {
                e.DumpLog (indent + 1);
            }
        }
    }

    public sealed class SyntaxTreeXXX : SyntaxTreeElement
    {
        public readonly Token1 Token;

        public SyntaxTreeXXX (Token1 token)
        {
            Token = token;
        }

        internal override void DumpLog (int indent)
        {
            _log (indent, $"{this.GetType ().Name}: {Token}");
        }
    }

    public abstract class SyntaxTreeElement
    {
        protected SyntaxTreeElement ()
        {
        }

        internal abstract void DumpLog (int indent);

        protected void _log (int indent, string value)
        {
            Log.Debug ($"{new string (' ', indent * 4) }- {value}");
        }

        protected static void _skipTokens (ReadOnlySpan<Token1> tokens, ref int i, params TokenType1 [] skip_token_types)
        {
            while (i < tokens.Length)
            {
                Token1 token = tokens [i];
                if (token.Type.IsAny (skip_token_types))
                {
                    i++;
                }
                else
                {
                    break;
                }
            }
        }

        protected static bool _detectBlock (ReadOnlySpan<Token1> tokens, string block_char_start, string block_char_stop, int i, out int block_index_start, out int block_index_stop)
        {
            Token1 token = tokens [i];
            if (token.Type == TokenType1.CONTROL_CHAR && token.Buffer == block_char_start)
            {
                i++;
                block_index_start = i;
                int in_braces = 0;
                while (i < tokens.Length)
                {
                    token = tokens [i];
                    if (token.Type == TokenType1.CONTROL_CHAR && token.Buffer == block_char_start)
                    {
                        in_braces++;
                    }
                    else if (token.Type == TokenType1.CONTROL_CHAR && token.Buffer == block_char_stop)
                    {
                        if (in_braces == 0)
                        {
                            block_index_stop = i;
                            return true;
                        }
                        else
                        {
                            in_braces--;
                        }
                    }
                    i++;
                }
            }

            block_index_start = 0;
            block_index_stop = 0;
            return false;
        }
    }
}
