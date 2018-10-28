using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;
using PHP.Standard;

namespace PHP.Tree
{
    public abstract class DeclarationExpression : Expression
    {

    }

    public sealed class FunctionDeclarationExpression : DeclarationExpression
    {
        public readonly Name Name;
        public readonly DeclarationSignature Signature;
        public readonly Expression Body;

        public FunctionDeclarationExpression (FunctionDecl e)
        {
            Name = e.Name;
            Signature = new DeclarationSignature (e.Signature);
            Body = Expressions.Parse (e.Body);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("parameters", Signature.Parameters),
                ("body", Body),
            };
        }

        protected override string GetTypeName ()
        {
            return $"function call: {Name}";
        }
    }

}
