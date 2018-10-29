using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class TryInterpreter
    {
        public static Result Run (TryExpression block, Scope scope)
        {
            Result body_result = Result.NULL;

            Expression body_expr = block.Body;
            if (body_expr != null)
            {
                body_result = Interpreters.Execute (body_expr, scope);
            }

            Expression finally_expr = block.Finally;
            if (finally_expr != null)
            {
                Interpreters.Execute (finally_expr, scope);
            }

            return body_result;
        }
    }


}
