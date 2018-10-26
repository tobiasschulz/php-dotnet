using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using Devsense.PHP.Errors;
using Devsense.PHP.Text;

namespace PHP
{
    public sealed class CodeContent
    {
        private readonly string _content_string;
        private readonly TokenizedContent _content_tokenized;
        private readonly SyntaxTree _syntax_tree;

        public CodeContent (string content_string)
        {
            _content_string = content_string;

            //_content_tokenized = Tokenizer.Tokenize (content_string);
            //_syntax_tree = SyntaxTreeAnalyzer.Analyze (_content_tokenized);
        }

        internal void Run ()
        {
            string a = @"


    $wrapper = Default_Wrapper::getInstance();
    $a = $wrapper->getMainWrapper()->getBaseModules();

";
            a = @"


    $wrapper = Default_WrappergetInstance()

";

            var sourceUnit = new CodeSourceUnit (a, "a", Encoding.UTF8, Lexer.LexicalStates.INITIAL, LanguageFeatures.Php71Set);
            var factory = new BasicNodesFactory (sourceUnit);
            var errors = new TestErrorSink ();

            GlobalCode ast = null;

                sourceUnit.Parse (factory, errors, new TestErrorRecovery ());
                ast = sourceUnit.Ast;

            Log.Debug (ast.Statements.Join(","));

            return;

            var res =   CodeParser.Instance.Value.ParseScript (a);
            Console.WriteLine (res);

            return;

            _syntax_tree.DumpLog ();

            Console.WriteLine ();

            foreach (Token t in _content_tokenized.Tokens)
            {
                Log.Debug (t);
            }
        }
        sealed internal class TestErrorSink : IErrorSink<Span2>
        {
            public class ErrorInstance
            {
                public Span2 Span;
                public ErrorInfo Error;
                public string [] Args;

                public override string ToString () => Error.ToString (Args);
            }

            public readonly List<ErrorInstance> Errors = new List<ErrorInstance> ();

            public int Count => this.Errors.Count;

            public void Error (Span2 span, ErrorInfo info, params string [] argsOpt)
            {
                Errors.Add (new ErrorInstance ()
                {
                    Span = span,
                    Error = info,
                    Args = argsOpt,
                });
            }
        }

        private class TestErrorRecovery : IErrorRecovery
        {
            bool IErrorRecovery.TryRecover (ILexerState lexerState)
            {
                return false;
            }
        }
    }
}
