namespace PHP.Tree
{
    public class NotExpression : Expression
    {
        public readonly Expression Value;

        public NotExpression (Expression value)
        {
            Value = value;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("", Value),
            };
        }

        protected override string GetTypeName ()
        {
            return $"not";
        }
    }
}
