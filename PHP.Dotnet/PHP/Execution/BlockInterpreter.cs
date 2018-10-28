using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class BlockInterpreter
    {
        public static Result Run (BlockExpression block, Scope scope)
        {
            return Run (block.Body, scope);
        }

        public static Result Run (ImmutableArray<Expression> expressions, Scope scope)
        {
            Result res = Result.NULL;
            foreach (Expression expression in expressions)
            {
                res = Interpreters.Run (expression, scope);
                if (res.FastReturn)
                {
                    return res;
                }
            }
            return res;
        }
    }


}
