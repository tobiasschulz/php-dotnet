using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class FunctionCallInterpreter
    {
        public static Result Run (FunctionCallExpression expression, Scope scope)
        {
            return Result.NULL;
        }

        public static Result Run (EchoExpression expression, Scope scope)
        {
            return Result.NULL;
        }
    }

}
