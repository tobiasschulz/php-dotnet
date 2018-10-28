using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using Devsense.PHP.Syntax;
using PHP.Parser;
using PHP.Tree;
using PHP.Execution;

namespace PHP
{
    public sealed class CodeContent
    {
        private readonly Context _context;
        private readonly string _content_string;
        private readonly PhpSyntaxTree _syntax_tree;

        public CodeContent (Context context, string content_string)
        {
            _context = context;
            _content_string = content_string;


            string a = @"
<?php

    $wrapper = Default_Wrapper::getInstance();
    $a = $wrapper->getMainWrapper()->getBaseModules();

";

            a = @"

    $wrapper = Default_Wrapper::getInstance();
    $a = $wrapper->getMainWrapper()->getBaseModules();

";

            a = content_string;

            _syntax_tree = PhpSyntaxTree.ParseCode (context, a, "a.php");
        }

        internal void Run (Scope previous_scope)
        {
            Console.WriteLine ("----------");

            Expression tree = Expressions.Parse (_syntax_tree.Root);
            tree.Print ();

            ScriptScope script_scope = new ScriptScope (previous_scope);
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
