using System;
using System.Collections.Generic;
using System.Text;
using PHP.Tree;

namespace PHP.Execution
{
    public sealed class Interpreters
    {
        public static Result Run (Expression expression, Scope scope)
        {
            if (expression == null)
            {
                return Result.NULL;
            }

            switch (expression)
            {
                case BlockExpression e:
                    return BlockInterpreter.Run (e, scope);

                case ConditionalBlockExpression e:
                    return ConditionalBlockInterpreter.Run (e, scope);

                case BinaryExpression e:
                    return BinaryInterpreter.Run (e, scope);

                case EchoExpression e:
                    return FunctionCallInterpreter.Run (e, scope);

                case FunctionCallExpression e:
                    return FunctionCallInterpreter.Run (e, scope);

                default:
                    Log.Error ($"Unable to execute expression: {expression}");
                    return Result.NULL;
            }
        }

    }
}
