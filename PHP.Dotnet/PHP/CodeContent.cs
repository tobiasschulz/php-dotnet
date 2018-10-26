using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PHP.Helper;
using PHP.Standard;

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


            string a = @"
<?php

    $wrapper = Default_Wrapper::getInstance();
    $a = $wrapper->getMainWrapper()->getBaseModules();

";
            //content_string = a;



            return;

            _content_tokenized = Tokenizer.Tokenize (content_string);
            _syntax_tree = SyntaxTreeAnalyzer.Analyze (_content_tokenized);

        }

        internal void Run ()
        {

            return;

            _content_tokenized.DumpLog ();

            Console.ReadLine ();

            _syntax_tree?.DumpLog ();

            Console.WriteLine ();

            foreach (RegularToken2 t in _content_tokenized.Tokens)
            {
                Log.Debug (t);
            }
        }
    }
}
