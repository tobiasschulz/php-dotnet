using System;
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

        public ConditionalBlockExpression (IfStmt e)
        {
            foreach (var c in e.Conditions)
            {
                if (c.Condition == null)
                {
                    if (Else == null)
                    {
                        Else = new ElseExpression (c);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException ($"Multiple else expressions ???");
                    }
                }
                else
                {
                    if (Ifs.Length == 0)
                    {
                        Ifs = Ifs.Add (new IfExpression (c));
                    }
                    else
                    {
                        Ifs = Ifs.Add (new ElseIfExpression (c));
                    }
                }
            }
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

        public ForeachExpression (ForeachStmt e)
        {
            Enumeree = Expressions.Parse (e.Enumeree);
            Body = Expressions.Parse (e.Body);
            KeyVariable = Expressions.Parse ((VarLikeConstructUse)e.KeyVariable?.Variable ?? e.KeyVariable?.List);
            ValueVariable = Expressions.Parse ((VarLikeConstructUse)e.ValueVariable?.Variable ?? e.ValueVariable?.List);
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

    public abstract class BaseIfExpression : Expression
    {
        public readonly Expression Condition;
        public readonly Expression Body;

        public BaseIfExpression (ConditionalStmt e)
        {
            Condition = Expressions.Parse (e.Condition);
            Body = Expressions.Parse (e.Statement);
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
        public IfExpression (ConditionalStmt e)
            : base (e)
        {
        }

        protected override string GetTypeName ()
        {
            return "if";
        }
    }

    public sealed class ElseIfExpression : BaseIfExpression
    {
        public ElseIfExpression (ConditionalStmt e)
            : base (e)
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

        public ElseExpression (ConditionalStmt e)
        {
            Body = Expressions.Parse (e.Statement);
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
