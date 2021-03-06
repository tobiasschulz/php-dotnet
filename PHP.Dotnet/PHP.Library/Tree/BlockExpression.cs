﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Standard;

namespace PHP.Tree
{
    public sealed class BlockExpression : Expression
    {
        public readonly ImmutableArray<Expression> Body;

        public BlockExpression (ImmutableArray<Expression> body)
        {
            Body = body;
        }
        
        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("", Body),
            };
        }

        protected override string GetTypeName ()
        {
            return "block";
        }
    }

    public sealed class ConditionalBlockExpression : Expression
    {
        public readonly ImmutableArray<BaseIfExpression> Ifs = ImmutableArray<BaseIfExpression>.Empty;
        public readonly ElseExpression Else;

        public ConditionalBlockExpression (ImmutableArray<BaseIfExpression> if_exprs, ElseExpression else_expr)
        {
            Ifs = if_exprs;
            Else = else_expr;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("", Ifs),
                ("", Else),
            };
        }

        protected override string GetTypeName ()
        {
            return "condition";
        }
    }

    public sealed class ForeachExpression : Expression
    {
        public readonly Expression Enumeree;
        public readonly Expression Body;
        public readonly Expression KeyVariable;
        public readonly Expression ValueVariable;

        public ForeachExpression (Expression enumeree, Expression body, Expression key_variable, Expression value_variable)
        {
            Enumeree = enumeree;
            Body = body;
            KeyVariable = key_variable;
            ValueVariable = value_variable;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("key", KeyVariable),
                ("value", ValueVariable),
                ("enumeree", Enumeree),
                ("body", Body),
            };
        }

        protected override string GetTypeName ()
        {
            return "foreach";
        }
    }

    public sealed class WhileExpression : Expression
    {
        public readonly Expression Condition;
        public readonly Expression Body;
        public readonly bool IsDoWhile;

        public WhileExpression (Expression condition, Expression body, bool is_do_while)
        {
            Condition = condition;
            Body = body;
            IsDoWhile = is_do_while;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("condition", Condition),
                ("body", Body),
            };
        }

        protected override string GetTypeName ()
        {
            return IsDoWhile ? "do-while" : "while";
        }
    }

    public abstract class BaseIfExpression : Expression
    {
        public readonly Expression Condition;
        public readonly Expression Body;

        public BaseIfExpression (Expression condition, Expression body)
        {
            Condition = condition;
            Body = body;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("condition", Condition),
                ("body", Body),
            };
        }
    }

    public sealed class IfExpression : BaseIfExpression
    {
        public IfExpression (Expression condition, Expression body)
            : base (condition, body)
        {
        }

        protected override string GetTypeName ()
        {
            return "if";
        }
    }

    public sealed class ElseIfExpression : BaseIfExpression
    {
        public ElseIfExpression (Expression condition, Expression body)
            : base (condition, body)
        {
        }

        protected override string GetTypeName ()
        {
            return "else if";
        }
    }

    public sealed class ElseExpression : Expression
    {
        public readonly Expression Body;

        public ElseExpression (Expression body)
        {
            Body = body;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("body", Body),
            };
        }

        protected override string GetTypeName ()
        {
            return "else";
        }
    }

}
