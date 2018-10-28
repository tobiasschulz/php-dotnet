using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class ConditionalBlockInterpreter
    {
        public static Result Run (ConditionalBlockExpression block, Scope scope)
        {
            foreach (BaseIfExpression if_expr in block.Ifs)
            {
                Result if_condition_result = Interpreters.Run (if_expr.Condition, scope);

                if (if_condition_result.IsTrue ())
                {
                    Result if_body_result = Interpreters.Run (if_expr.Body, scope);
                    return if_body_result;
                }
            }

            ElseExpression else_expr = block.Else;
            if (else_expr != null)
            {
                Result else_body_result = Interpreters.Run (else_expr.Body, scope);
                return else_body_result;
            }

            return Result.NULL;
        }
    }


}
