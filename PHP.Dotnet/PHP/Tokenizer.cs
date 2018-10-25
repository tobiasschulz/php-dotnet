using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using System.Collections.Immutable;

namespace PHP
{
    public static class Tokenizer
    {
        internal static char [] OpenTagLong = "<?php".ToCharArray ();
        internal static char [] OpenTagShort = "<?".ToCharArray ();
        internal static char [] CloseTag = "?>".ToCharArray ();

        internal static char [] CommentBlockStart = "/*".ToCharArray ();
        internal static char [] CommentBlockStop = "*/".ToCharArray ();

        internal static char [] CommentLineStart1 ="//".ToCharArray ();
        internal static char [] CommentLineStart2= "#".ToCharArray ();

        internal static char [] ArrowArray = "=>".ToCharArray ();
        internal static char [] ArrowCall = "->".ToCharArray ();
        internal static char [] DoubleColon = "::".ToCharArray ();

        internal static char [] CharsControlSyntax = "(){}[]<>=-,;+/\\*:".ToCharArray ();

        internal static TokenizedContent Tokenize (string content_string)
        {
            ReadOnlyMemory<char> content_memory = content_string.AsMemory ();
            ReadOnlySpan<char> content_span = content_memory.Span;
            List<Token> tokens = new List<Token> ();

            int length = content_span.Length;
            int token_index_start = 0;
            bool is_outside = true;
            bool is_inside_string = false;
            char is_inside_string_end = (char)0;
            bool is_inside_comment_block = false;
            bool is_inside_comment_line = false;
            bool is_inside_identifier = false;

            for (int i = 0; i < length;)
            {
                if (content_span.Matches (i, CloseTag))
                {
                    int token_index_stop = i;
                    tokens.Add (_makeToken (TokenType.OUTSIDE, token_index_start, token_index_stop, content_memory));
                    i += CloseTag.Length;
                    is_outside = true;
                    continue;
                }

                if (content_span.Matches (i, OpenTagLong))
                {
                    i += OpenTagLong.Length;
                    token_index_start = i;
                    is_outside = false;
                    continue;
                }

                if (content_span.Matches (i, OpenTagShort))
                {
                    i += OpenTagShort.Length;
                    token_index_start = i;
                    is_outside = false;
                    continue;
                }

                if (is_outside)
                {
                    i++;

                }
                else
                {
                    char c = content_span [i];

                    if (!is_inside_comment_block && content_span.Matches (i, CommentBlockStart))
                    {
                        token_index_start = i;
                        i += CommentBlockStart.Length;
                        is_inside_comment_block = true;
                    }
                    else if (!is_inside_comment_line && content_span.Matches (i, CommentLineStart1))
                    {
                        token_index_start = i;
                        i += CommentLineStart1.Length;
                        is_inside_comment_line = true;
                    }
                    else if (!is_inside_comment_line && content_span.Matches (i, CommentLineStart2))
                    {
                        token_index_start = i;
                        i += CommentLineStart2.Length;
                        is_inside_comment_line = true;
                    }
                    else if (is_inside_comment_block)
                    {
                        if (content_span.Matches (i, CommentBlockStop))
                        {
                            i += CommentBlockStop.Length;
                            int token_index_stop = i;
                            tokens.Add (_makeToken (TokenType.COMMENT, token_index_start, token_index_stop, content_memory));
                            is_inside_comment_block = false;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else if (is_inside_comment_line)
                    {
                        if (c == '\n' || c == '\r')
                        {
                            i += 1;
                            int token_index_stop = i;
                            tokens.Add (_makeToken (TokenType.COMMENT, token_index_start, token_index_stop, content_memory));
                            is_inside_comment_line = false;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else if (is_inside_string)
                    {
                        if (c == '\\')
                        {
                            i += 2;
                        }
                        else if (c == is_inside_string_end)
                        {
                            int token_index_stop = i;
                            tokens.Add (_makeToken (TokenType.STRING, token_index_start, token_index_stop, content_memory));
                            is_inside_string = false;
                            i++;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else
                    {
                        if (c == '"')
                        {
                            is_inside_string = true;
                            is_inside_string_end = '"';
                            i++;
                            token_index_start = i;
                        }
                        else if (c == '\'')
                        {
                            is_inside_string = true;
                            is_inside_string_end = '\'';
                            i++;
                            token_index_start = i;
                        }
                        else if (c.IsAny (CharsControlSyntax))
                        {
                            if (is_inside_identifier)
                            {
                                int token_index_stop = i;
                                tokens.Add (_makeToken (TokenType.IDENTIFIER, token_index_start, token_index_stop, content_memory));
                                is_inside_identifier = false;
                            }

                            if (content_span.Matches (i, ArrowArray))
                            {
                                int token_index_stop = i;
                                tokens.Add (_makeToken (TokenType.ARROW_ARRAY));
                                i += ArrowArray.Length;
                            }
                            else if (content_span.Matches (i, ArrowCall))
                            {
                                int token_index_stop = i;
                                tokens.Add (_makeToken (TokenType.ARROW_CALL));
                                i += ArrowCall.Length;
                            }
                            else if (content_span.Matches (i, DoubleColon))
                            {
                                int token_index_stop = i;
                                tokens.Add (_makeToken (TokenType.DOUBLE_COLON));
                                i += DoubleColon.Length;
                            }
                            else
                            {
                                tokens.Add (_makeToken (TokenType.CONTROL_CHAR, c));

                                token_index_start = i;
                                i++;
                            }
                        }
                        else if (char.IsWhiteSpace (c))
                        {
                            if (is_inside_identifier)
                            {
                                int token_index_stop = i;
                                tokens.Add (_makeToken (content_span [token_index_start] == '$' ? TokenType.VARIABLE : TokenType.IDENTIFIER, token_index_start, token_index_stop, content_memory));
                                is_inside_identifier = false;
                            }
                            if (tokens.Count == 0 || tokens [tokens.Count - 1].Type != TokenType.SPACE)
                            {
                                //tokens.Add (_makeToken (TokenType.SPACE));
                            }
                            i++;
                        }
                        else if (!is_inside_identifier && _is_identifier_char (c))
                        {
                            is_inside_identifier = true;
                            token_index_start = i;
                        }
                        else
                        {

                            i++;

                        }
                    }
                }
            }

            return new TokenizedContent (
                tokens.ToImmutableArray ()
            );
        }

        private static Token _makeToken (TokenType type, int i_start, int i_stop, ReadOnlyMemory<char> s)
        {
            return new Token (type, s.Slice (i_start, i_stop - i_start).Span.ToString ());
        }

        private static Token _makeToken (TokenType type, char c)
        {
            return new Token (type, c.ToString ());
        }

        private static Token _makeToken (TokenType type)
        {
            return new Token (type, string.Empty);
        }

        private static bool _is_identifier_char (char value)
        {
            return value == '$' || value == '_' || char.IsLetterOrDigit (value);
        }

        public static bool Matches (this ReadOnlySpan<char> haystack, int offset, char [] needle)
        {
            if (haystack.Length >= offset + needle.Length)
            {
                for (int i = 0; i < needle.Length; i++)
                {
                    if (haystack [offset + i] != needle [i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
