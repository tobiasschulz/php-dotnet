﻿using System.Collections.Generic;
using System.Text;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;

namespace PHP.Tree
{
    public abstract class LikeVariable : Expression
    {
    }

    public sealed class Variable : LikeVariable
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

        public Variable (Name name)
        {
            Name = new VariableName (name.Value);
        }

        protected override string GetTypeName ()
        {
            return $"variable: {Name}";
        }
    }

}
