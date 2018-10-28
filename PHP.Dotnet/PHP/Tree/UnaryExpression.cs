using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;

namespace PHP.Tree
{
    public class UnaryExpression : Expression
    {
        public readonly Expression Value;
        public readonly UnaryOp Operation;

        public UnaryExpression (Expression value, UnaryOp operation)
        {
            Value = value;
            Operation = operation;
        }

        public UnaryExpression (Devsense.PHP.Syntax.Ast.Expression value, UnaryOp operation)
        {
            Value = Expressions.Parse (value);
            Operation = operation;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("", Value),
            };
        }

        protected override string GetTypeName ()
        {
            return $"unary: {Operation}";
        }
    }

    public enum UnaryOp
    {
        StringCast,
        CAST_STRING
    }
}
