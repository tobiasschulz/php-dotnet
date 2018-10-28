using System.Collections.Immutable;
using System.Linq;
using Devsense.PHP.Syntax.Ast;

namespace PHP.Tree
{
    public sealed class ArrayExpression : LikeVariable
    {
        public readonly ImmutableArray<ArrayItemExpression> Items;

        public ArrayExpression (ArrayEx e)
        {
            Items = e.Items.Select (i =>
            {
                if (i == null) return null;
                switch (i)
                {
                    case ValueItem v:
                        return (ArrayItemExpression)new ArrayItemValueExpression (v);
                    case RefItem v:
                        return (ArrayItemExpression)new ArrayItemRefExpression (v);
                    default:
                        Log.Error ($"Unknown kind of array item: {i}");
                        return null;
                }
            }).Where (i => i != null).ToImmutableArray ();
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("", Items),
            };
        }

        protected override string GetTypeName ()
        {
            return $"array";
        }
    }

    public abstract class ArrayItemExpression : LikeVariable
    {
        public readonly Expression Key;
        public readonly Expression Value;

        protected ArrayItemExpression (Expression key, Expression value)
        {
            Key = key;
            Value = value;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("key", Key),
                ("value", Value),
            };
        }

    }

    public sealed class ArrayItemValueExpression : ArrayItemExpression
    {
        public ArrayItemValueExpression (ValueItem e)
            : base (Expressions.Parse (e.Index), Expressions.Parse (e.ValueExpr))
        {
        }

        protected override string GetTypeName ()
        {
            return $"item (value)";
        }
    }

    public sealed class ArrayItemRefExpression : ArrayItemExpression
    {
        public ArrayItemRefExpression (RefItem e)
            : base (Expressions.Parse (e.Index), Expressions.Parse (e.RefToGet))
        {
        }

        protected override string GetTypeName ()
        {
            return $"item (ref)";
        }
    }

}
