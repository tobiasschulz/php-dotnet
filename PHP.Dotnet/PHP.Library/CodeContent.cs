using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Standard;
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
        private readonly IParseResult _parse_result;

        public CodeContent (Context context, string content_string, IScript script)
        {
            _context = context;
            _content_string = content_string;
            _script = script;

            _parse_result = _context.Parser.ParseCode (context, content_string, script.GetScriptPath ().Original);
        }

        public string Eval (Scope previous_scope, ContextOptions options, List<string> diagnostics = null)
        {
            Expression tree = _parse_result.RootExpression;
            tree.Print ();
            ScriptScope script_scope = new ScriptScope (previous_scope, _script);

            if (_parse_result.Diagnostics.Length != 0)
            {
                if (diagnostics != null)
                    foreach (var diag in _parse_result.Diagnostics)
                        diagnostics.Add (diag.ToString ());
                else

                    throw new Exception (_parse_result.Diagnostics.Join ("\n"));
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

                Expression tree = _parse_result.RootExpression;
                tree.Print ();

                ScriptScope script_scope = new ScriptScope (previous_scope, _script);
                Interpreters.Execute (tree, script_scope);

                Console.WriteLine ("----------");

                foreach (var diag in _parse_result.Diagnostics)
                {
                    Log.Debug (diag);
                }

                Console.ReadLine ();
            }
            else
            {
                Expression tree = _parse_result.RootExpression;
                //tree.Print ();

                ScriptScope script_scope = new ScriptScope (previous_scope, _script);
                Interpreters.Execute (tree, script_scope);

                foreach (var diag in _parse_result.Diagnostics)
                {
                    Log.Debug (diag);
                }
            }
        }
    }
}
