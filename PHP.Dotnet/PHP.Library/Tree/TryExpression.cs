using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Library.TypeSystem;
using PHP.Standard;

namespace PHP.Tree
{
    public sealed class TryExpression : Expression
    {
        public readonly Expression Body;
        public readonly ImmutableArray<CatchExpression> Catches;
        public readonly Expression Finally;

        public TryExpression (Expression body, ImmutableArray<CatchExpression> catches, Expression @finally)
        {
            Body = body;
            Catches = catches;
            Finally = @finally;
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
        public readonly Expression ClassName;

        public CatchExpression (Expression body, VariableExpression variable, Expression class_name)
        {
            Body = body;
            Variable = variable;
            ClassName = class_name;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("class_name", ClassName),
                ("variable", Variable),
                ("body", Body),
            };
        }

        protected override string GetTypeName ()
        {
            return $"catch";
        }
    }

}
