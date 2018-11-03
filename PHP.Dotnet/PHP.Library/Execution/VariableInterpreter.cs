using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Library.TypeSystem;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class VariableInterpreter
    {
        public static Result Run (VariableExpression variable_expression, Scope scope)
        {
            scope.Variables.EnsureExists (variable_expression.Name, out IVariable variable);
            return new Result (variable.Value);
        }
        
        public static Result Run (PseudoConstExpression pseudo_const_expression, Scope scope)
        {
            switch (pseudo_const_expression.Type)
            {
                case PseudoConstExpression.PseudoConstType.FILE:
                    {
                        string value = null;
                        scope.FindNearestScope<ScriptScope> (ss =>
                        {
                            value = ss.Script.GetScriptPath ().Original;
                        });
                        return new Result (new StringExpression (value));
                    }

                case PseudoConstExpression.PseudoConstType.FUNCTION:
                    {
                        string value = null;
                        scope.FindNearestScope<FunctionScope> (fs =>
                        {
                            value = fs.Function.Name.Value;
                        });
                        return new Result (new StringExpression (value));
                    }

                default:
                    Log.Error ($"Pseudo constant is not implemented: {pseudo_const_expression.Type}, scope: {scope}");
                    break;
            }
            scope.Variables.EnsureExists (new VariableName (pseudo_const_expression.Type.ToString ()), out IVariable variable);
            return new Result (variable.Value);
        }
        
    }

}
