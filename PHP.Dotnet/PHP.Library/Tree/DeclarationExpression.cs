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
        public readonly DeclarationSignature DeclarationSignature;
        public readonly Expression Body;

        public FunctionDeclarationExpression (FunctionDecl e)
        {
            Name = e.Name;
            DeclarationSignature = new DeclarationSignature (e.Signature);
            Body = Expressions.Parse (e.Body);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("parameters", DeclarationSignature.Parameters),
                ("body", Body),
            };
        }

        protected override string GetTypeName ()
        {
            return $"function declaration: {Name}";
        }
    }

}
