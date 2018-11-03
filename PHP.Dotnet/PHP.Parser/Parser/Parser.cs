using System;
using System.Collections.Generic;
using System.Text;
using Devsense.PHP.Syntax.Ast;
using PHP.Tree;

namespace PHP.Parser
{
    public sealed class Parser : IParser
    {
        IParseResult IParser.ParseCode (Context context, string content_string, string filename)
        {
            return SyntaxTree.ParseCode (this, context, content_string, filename);
        }

        Tree.Expression IParser.Parse (object obj)
        {
            return Parse (obj as LangElement);
        }

        public Tree.Expression Parse (LangElement element)
        {
            return Expressions.Parse (element);
        }
    }
}
