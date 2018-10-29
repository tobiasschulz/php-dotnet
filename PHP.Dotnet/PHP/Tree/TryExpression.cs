using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;
using PHP.Standard;

namespace PHP.Tree
{
    public sealed class TryExpression : Expression
    {
        public readonly Expression Body;
        public readonly ImmutableArray<CatchExpression> Catches;
        public readonly Expression Finally;

        public TryExpression (TryStmt e)
        {
            Body = Expressions.Parse (e.Body);
            Catches = e.Catches.Select (c => new CatchExpression (c)).ToImmutableArray ();
            Finally = Expressions.Parse (e.FinallyItem?.Body);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("body", Body),
                ("catches", Catches),
                ("finally", Finally),
            };
        }

        protected override string GetTypeName ()
        {
            return "try";
        }
    }

    public sealed class CatchExpression : Expression
    {
        public readonly Expression Body;
        public readonly VariableExpression Variable;
        public readonly TypeRef TargetType;

        public CatchExpression (CatchItem e)
        {
            Body = Expressions.Parse (e.Body);
            Variable = new VariableExpression (e.Variable);
            TargetType = e.TargetType;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("variable", Variable),
                ("body", Body),
            };
        }

        protected override string GetTypeName ()
        {
            return $"catch: {TargetType}";
        }
    }

}
