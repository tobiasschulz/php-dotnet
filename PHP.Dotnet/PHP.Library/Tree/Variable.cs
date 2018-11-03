using System.Collections.Generic;
using System.Text;
using PHP.Library.TypeSystem;

namespace PHP.Tree
{
    public abstract class LikeVariable : Expression
    {
    }

    public sealed class VariableExpression : LikeVariable
    {
        public readonly NameOfVariable Name;

        public VariableExpression (NameOfVariable name)
        {
            Name = name;
        }
        
        protected override string GetTypeName ()
        {
            return $"variable: {Name}";
        }
    }

}
