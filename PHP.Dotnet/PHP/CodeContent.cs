using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using Devsense.PHP.Syntax;
using PHP.Parser;
using PHP.Tree;

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

        internal void Run ()
        {
            Console.WriteLine ("----------");

            Expression tree = Expressions.Parse (_syntax_tree.Root);
            tree.Print ();

            Console.WriteLine ("----------");

            foreach (var diag in _syntax_tree.Diagnostics)
            {
                Log.Debug (diag);
            }

            foreach (var type in _syntax_tree.Types)
            {
                Log.Debug (type);
            }
            foreach (var func in _syntax_tree.Functions)
            {
                Log.Debug (func);
            }
            foreach (var y in _syntax_tree.YieldNodes)
            {
                Log.Debug (y);
            }

            Log.Debug (_syntax_tree.Root);

            foreach (var y in _syntax_tree.Root.Statements)
            {
                Log.Debug (y);
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
