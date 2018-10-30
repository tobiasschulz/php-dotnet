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

        public string Eval (Scope previous_scope, out string [] diagnostics)
        {
            Expression tree = Expressions.Parse (_syntax_tree.Root);
            tree.Print ();
            ScriptScope script_scope = new ScriptScope (previous_scope, _script);
            List<string> diags = new List<string> ();
            foreach (var diag in _syntax_tree.Diagnostics)
            {
                diags.Add (diag.ToString ());
            }
            string res = Interpreters.Execute (tree, script_scope).ResultValue.GetStringValue ();
            diagnostics = diags.ToArray ();
            return res;
        }

        public string Eval (Scope previous_scope)
        {
            string res = Eval (previous_scope, out var diagnostics);
            if (diagnostics.Length != 0)
            {
                throw new Exception (diagnostics.Join ("\n"));
            }
            return res;
        }

        public void Run (Scope previous_scope)
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
