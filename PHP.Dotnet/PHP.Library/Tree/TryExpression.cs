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
        public readonly NameOfClass TargetType;

        public CatchExpression (Expression body, VariableExpression variable, NameOfClass target_type)
        {
            Body = body;
            Variable = variable;
            TargetType = target_type;
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
