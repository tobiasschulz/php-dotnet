using System.Collections.Immutable;
using System.Linq;

namespace PHP.Tree
{
    public sealed class ArrayExpression : LikeVariable
    {
        public readonly ImmutableArray<ArrayItemExpression> Items;

        public ArrayExpression (ImmutableArray<ArrayItemExpression> items)
        {
            Items = items;
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

    public sealed class ArrayAccessExpression : Expression
    {
        public readonly Expression Array;
        public readonly Expression Index;

        public ArrayAccessExpression (Expression array, Expression index)
        {
            Array = array;
            Index = index;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("array", Array),
                ("index", Index),
            };
        }

        protected override string GetTypeName ()
        {
            return $"array access";
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
        public ArrayItemValueExpression (Expression key, Expression value)
            : base (key, value)
        {
        }

        protected override string GetTypeName ()
        {
            return $"item (value)";
        }
    }

    public sealed class ArrayItemRefExpression : ArrayItemExpression
    {
        public ArrayItemRefExpression (Expression key, Expression value)
            : base (key, value)
        {
        }

        protected override string GetTypeName ()
        {
            return $"item (ref)";
        }
    }

}
