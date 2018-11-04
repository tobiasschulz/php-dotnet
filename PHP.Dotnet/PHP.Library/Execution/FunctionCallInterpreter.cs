using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Library.Internal;
using PHP.Library.TypeSystem;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class FunctionCallInterpreter
    {
        public static Result Run (FunctionCallExpression expression, Scope scope)
        {
            if (scope.Root.Functions.TryGetValue (expression.Name, out IFunction function))
            {
                try
                {
                    return function.Execute (new EvaluatedCallSignature (expression.CallSignature, scope), scope);
                }
                catch (ReturnException ex)
                {
                    return new Result (ex.ReturnValue);
                }
            }
            else
            {
                Log.Error ($"Function could not be found: {expression.Name}, scope: {scope}");
                Log.Error ($"  existing functions: {scope.Root.Functions.GetAll ().Select (f => f.Name).Join (", ")}");
                return Result.NULL;
            }
        }

        public static Result Run (EchoExpression expression, Scope scope)
        {
            return Result.NULL;
        }
    }

}
