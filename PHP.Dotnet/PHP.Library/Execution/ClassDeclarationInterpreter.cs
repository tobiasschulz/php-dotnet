using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Library.TypeSystem;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class ClassDeclarationInterpreter
    {
        public static Result Run (ClassDeclarationExpression expression, Scope scope)
        {
            scope.Root.Classes.Add (new InterpretedClass (expression));

            return Result.NULL;
        }

        private sealed class InterpretedClass : IClass
        {
            private readonly NameOfClass _name;
            private readonly ClassDeclarationExpression _expression;

            public InterpretedClass (ClassDeclarationExpression expression)
            {
                _name = expression.Name;
                _expression = expression;
            }

            NameOfClass IClass.Name => _name;


        }
    }

}
