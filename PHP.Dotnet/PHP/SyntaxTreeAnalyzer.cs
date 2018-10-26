using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace PHP
{
    public class SyntaxTreeAnalyzer
    {
        internal static SyntaxTree Analyze (TokenizedContent content_tokenized)
        {
            ReadOnlySpan<Token> tokens = content_tokenized.Tokens.ToArray ().AsSpan ();

            ImmutableArray<SyntaxTreeElement> elements = Analyze<object> (tokens, new Type [0]);

            return new SyntaxTree (
                elements.ToImmutableArray ()
            );
        }

        internal static ImmutableArray<SyntaxTreeElement> Analyze<TOuterElement> (ReadOnlySpan<Token> tokens, Type [] skip_types = null, bool allow_array_entry = false, bool all_complex_skipped = false)
        {
            if (typeof (TOuterElement) == typeof (SyntaxTreeArray))
            {
                Log.Debug ($"Analyze for {typeof (TOuterElement).Name}: (allow_array_entry = {allow_array_entry})");
                foreach (var t in tokens) Log.Debug ($"  - {t}");
                Log.Debug ("");
            }

            bool is_skipped_type (Type t)
            {
                if (all_complex_skipped) return true;
                return typeof (TOuterElement) == t || (skip_types != null && skip_types.Contains (t));
            }

            List<SyntaxTreeElement> elements = new List<SyntaxTreeElement> ();
            List<SyntaxTreeElement> stack = new List<SyntaxTreeElement> ();

            void tryPutStack ()
            {
                elements.AddRange (stack); // new SyntaxTreeStatement (stack.ToImmutableArray ()));
                stack.Clear ();
            }

            for (int i = 0; i < tokens.Length;)
            {
                Token token = tokens [i];

                if (!is_skipped_type (typeof (SyntaxTreeArrayEntry)) && allow_array_entry && SyntaxTreeArrayEntry.TryDetect (tokens, offset: ref i, out var result_array_entry))
                {
                    tryPutStack ();
                    elements.Add (result_array_entry);
                }
                else if (!is_skipped_type (typeof (SyntaxTreeTry)) && SyntaxTreeTry.TryDetect (tokens, offset: ref i, out var result_try))
                {
                    tryPutStack ();
                    elements.Add (result_try);
                }
                else if (!is_skipped_type (typeof (SyntaxTreeStatement)) && SyntaxTreeStatement.TryDetect (tokens, offset: ref i, out var result_statement))
                {
                    tryPutStack ();
                    elements.Add (result_statement);
                }
                else if (!is_skipped_type (typeof (SyntaxTreeAssignment)) && SyntaxTreeAssignment.TryDetect (tokens, offset: ref i, out var result_assignment))
                {
                    tryPutStack ();
                    elements.Add (result_assignment);
                }
                else if (!is_skipped_type (typeof (SyntaxTreeArray)) && SyntaxTreeArray.TryDetect (tokens, offset: ref i, out var result_array))
                {
                    tryPutStack ();
                    elements.Add (result_array);
                }
                else if (!is_skipped_type (typeof (SyntaxTreeCall)) && SyntaxTreeCall.TryDetect (tokens, offset: ref i, out var result_call))
                {
                    tryPutStack ();
                    elements.Add (result_call);
                }
                else
                {
                    stack.Add (new SyntaxTreeXXX (token));
                    i++;
                }
            }

            if (stack.Count != 0)
            {
                elements.AddRange (stack); // new SyntaxTreeStatement (stack.ToImmutableArray ()));
                stack.Clear ();
            }

            return elements.ToImmutableArray ();
        }
    }
}
