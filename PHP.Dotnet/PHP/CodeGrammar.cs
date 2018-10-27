using System;
using System.Collections.Generic;
using System.Text;
using Eto.Parse;
using Eto.Parse.Parsers;
using PHP.Standard;

namespace PHP
{
    public sealed class CodeGrammar
    {
        private readonly Grammar _grammar;

        public CodeGrammar ()
        {
            var jstring = new StringParser { AllowEscapeCharacters = true, Name = "string" };
            var jnumber = new NumberParser { AllowExponent = true, AllowSign = true, AllowDecimal = true, Name = "number" };
            var jboolean = new BooleanTerminal { Name = "bool", TrueValues = new string [] { "true" }, FalseValues = new string [] { "false" }, CaseSensitive = false };
            var jname = new StringParser { AllowEscapeCharacters = true, Name = "name" };
            var jnull = new LiteralTerminal { Value = "null", Name = "null", CaseSensitive = false };
            var ws = new RepeatCharTerminal (char.IsWhiteSpace);

            var commaDelimiter = new RepeatCharTerminal (new RepeatCharItem (char.IsWhiteSpace), ',', new RepeatCharItem (char.IsWhiteSpace));

            var scalar = jstring | jnumber | jboolean | jnull;

            var expression = new SequenceParser { Name = "object" };

            var math_plus = expression & "+" & expression;

            expression.Add (math_plus);
            expression.Add (scalar);
            if (false)
            {

                expression.Add (scalar);
                expression.Add (ws & expression & ws);
                expression.Add ("(", expression, ")");
                expression.Add (expression, "+", expression);
                expression.Add (expression, "-", expression);
                expression.Add (expression, "*", expression);
                expression.Add (expression, "/", expression);

            }

            /*
            // nonterminals (things we're interested in getting back)
            var jobject = new SequenceParser { Name = "object" };
            var jarray = new SequenceParser { Name = "array" };
            var jprop = new SequenceParser { Name = "property" };

            // rules
            var jvalue = jstring | jnumber | jobject | jarray | jboolean | jnull;
            jobject.Add ("{", (-jprop).SeparatedBy (commaDelimiter), "}");
            jprop.Add (jname, ":", jvalue);
            jarray.Add ("[", (-jvalue).SeparatedBy (commaDelimiter), "]");

            var expression = ws & (jobject | jarray) & ws;
            */

            // separate sequence and repeating parsers by whitespace
            expression.SeparateChildrenBy (ws, false);

            // allow whitespace before and after the initial object or array
            // var expression = ws & (jobject | jarray) & ws;

            // our grammar
            _grammar = new Grammar (
                  ws & expression & ws
            );
        }

        internal void Parse (string value)
        {
            GrammarMatch match = _grammar.Match (value);

            Log.Debug (match.ErrorMessage);
            Log.Debug (match.Matches.ToJson ());
        }
    }
}
