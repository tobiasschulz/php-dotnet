using System.Collections.Generic;
using System.Text;
using Devsense.PHP.Syntax.Ast;

namespace PHP.Tree
{
    public sealed class Variable : Expression
    {
        public readonly VariableName Name;

        public Variable (VariableName name)
        {
            Name = name;
        }

        public Variable (DirectVarUse variable)
        {
            Name = variable.VarName;
        }

        protected override string GetTypeName ()
        {
            return "variable";
        }
    }

}
