using System.Collections.Generic;
using System.Text;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;

namespace PHP.Tree
{
    public abstract class LikeVariable : Expression
    {
    }

    public sealed class VariableExpression : LikeVariable
    {
        public readonly VariableName Name;

        public VariableExpression (VariableName name)
        {
            Name = name;
        }

        public VariableExpression (DirectVarUse variable)
        {
            Name = variable.VarName;
        }

        public VariableExpression (Name name)
        {
            Name = new VariableName (name.Value);
        }

        protected override string GetTypeName ()
        {
            return $"variable: {Name}";
        }
    }

}
