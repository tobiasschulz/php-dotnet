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
    internal class TokenContraction_Step2
    {
        private static string [] _operator_precedence = new [] { "*", "/", "%", "+", "-", ".", "&", "^", "|", "&&", "||", "?:", "??", "=>", ",", ";", };

        internal static RegularToken2 GroupOperators (RegularToken2 input_tok)
        {
            RegularToken2 output_tok = new RegularToken2 (input_tok.Type, input_tok.Buffer);

            IReadOnlyList<RegularToken2> output_children = input_tok.Children.Select (input_child => GroupOperators ((RegularToken2)input_child)).ToImmutableArray ();

            ImmutableHashSet<string> used_operators = output_children.Where (t => t.Type == TokenType2.OPERATOR).Select (t => t.Buffer).ToImmutableHashSet();
            ImmutableArray<string> operators_for_grouping = _operator_precedence.Where (op => used_operators.Contains (op)).Reverse ().ToImmutableArray ();

            output_children = _groupChildrenByOperators (output_children, operators_for_grouping);

            foreach (RegularToken2 child in output_children)
            {
                output_tok.Add (child);
            }

            return output_tok;
        }

        private static IReadOnlyList<RegularToken2> _groupChildrenByOperators (IReadOnlyList<RegularToken2> children, IEnumerable<string> operators_for_grouping)
        {
            string op = operators_for_grouping.FirstOrDefault ();
            if (op == null)
            {
                return children;
            }

            if (children.Count == 0 || children.None (t => t.Type == TokenType2.OPERATOR && t.Buffer == op))
            {
                return children;
            }

            RegularToken2 op_group = new RegularToken2 (TokenType2.OPERATOR, op);
            List<RegularToken2> part = new List<RegularToken2> ();

            void submit_part ()
            {
                if (part.Count != 0)
                {
                    IReadOnlyList<RegularToken2> part_further_processed = _groupChildrenByOperators (part, operators_for_grouping.Skip (1));
                    if (part_further_processed.Count >= 2)
                    {
                        RegularToken2 part_token = new RegularToken2 (TokenType2.GROUP, string.Empty);
                        foreach (var c in part_further_processed)
                        {
                            part_token.Add (c);
                        }
                        op_group.Add (part_token);
                    }
                    else if (part_further_processed.Count == 1)
                    {
                        op_group.Add (part_further_processed.First ());
                    }
                    part.Clear ();
                }
            }

            foreach (RegularToken2 child in children)
            {
                if (child.Type == TokenType2.OPERATOR && child.Buffer == op)
                {
                    submit_part ();
                }
                else
                {
                    part.Add (child);
                }
            }

            submit_part ();

            return new [] { op_group };
        }
    }
}
