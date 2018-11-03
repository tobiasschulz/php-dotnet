using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Standard;
using Devsense.PHP.Syntax;
using PHP.Parser;
using PHP.Tree;
using PHP.Execution;
using System.Linq;
using PHP.Library.TypeSystem;

namespace PHP
{
    public sealed class CodeContent
    {
        private readonly Context _context;
        private readonly string _content_string;
        private readonly IScript _script;
        private readonly PhpSyntaxTree _syntax_tree;

        public CodeContent (Context context, string content_string, IScript script)
        {
            _context = context;
            _content_string = content_string;
            _script = script;

            _syntax_tree = PhpSyntaxTree.ParseCode (context, content_string, script.GetScriptPath ().Original);
        }

        public string Eval (Scope previous_scope, ContextOptions options, List<string> diagnostics = null)
        {
            Expression tree = Expressions.Parse (_syntax_tree.Root);
            tree.Print ();
            ScriptScope script_scope = new ScriptScope (previous_scope, _script);

            if (_syntax_tree.Diagnostics.Length != 0)
            {
                if (diagnostics != null)
                    foreach (var diag in _syntax_tree.Diagnostics)
                        diagnostics.Add (diag.ToString ());
                else
                    throw new Exception (_syntax_tree.Diagnostics.Join ("\n"));
            }

            string res = Interpreters.Execute (tree, script_scope).ResultValue.GetStringValue ();

            if (options.DEBUG_EXECUTION)
            {
                Console.ReadLine ();
            }

            return res;
        }

        public void Run (Scope previous_scope, ContextOptions options)
        {
            if (options.DEBUG_EXECUTION)
            {
                Console.WriteLine ("----------");

                Expression tree = Expressions.Parse (_syntax_tree.Root);
                tree.Print ();

                ScriptScope script_scope = new ScriptScope (previous_scope, _script);
                Interpreters.Execute (tree, script_scope);

                Console.WriteLine ("----------");

                foreach (var diag in _syntax_tree.Diagnostics)
                {
                    Log.Debug (diag);
                }

                Console.ReadLine ();
            }
            else
            {
                Expression tree = Expressions.Parse (_syntax_tree.Root);
                tree.Print ();

                ScriptScope script_scope = new ScriptScope (previous_scope, _script);
                Interpreters.Execute (tree, script_scope);

                foreach (var diag in _syntax_tree.Diagnostics)
                {
                    Log.Debug (diag);
                }
            }

            /*
            _content_tokenized.DumpLog ();

            Console.ReadLine ();

            _syntax_tree?.DumpLog ();

            Console.WriteLine ();

            foreach (RegularToken2 t in _content_tokenized.Tokens)
            {
                Log.Debug (t);
            }
            */
        }
    }
}
