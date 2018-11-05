using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace PHP.Tree
{
    public class BinaryExpression : Expression
    {
        public readonly Expression Left;
        public readonly Expression Right;
        public readonly BinaryOp Operation;

        public BinaryExpression (Expression left, Expression right, BinaryOp operation)
        {
            Left = left;
            Right = right;
            Operation = operation;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("left", Left),
                ("right", Right)
            };
        }

        protected override string GetTypeName ()
        {
            return $"binary: {Operation}";
        }
    }

    public enum BinaryOp
    {
        MUL,
        DIV,
        ADD,
        SUB,

        OR,
        AND,

        CONCAT,
        EQUAL,
        IDENTICAL,
        LESS_THAN,
    }
}
