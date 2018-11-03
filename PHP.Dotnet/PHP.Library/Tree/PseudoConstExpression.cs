namespace PHP.Tree
{
    public sealed class PseudoConstExpression : Expression
    {
        public readonly PseudoConstType Type;

        public PseudoConstExpression (PseudoConstType type)
        {
            Type = type;
        }

        protected override string GetTypeName ()
        {
            return $"pseudo const: {Type}";
        }

        public enum PseudoConstType
        {
            LINE = 0,
            FILE = 1,
            CLASS = 2,
            TRAIT = 3,
            FUNCTION = 4,
            METHOD = 5,
            NAMESPACE = 6,
            DIR = 7,
        }
    }
}
