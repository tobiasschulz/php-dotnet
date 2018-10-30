using System;
using System.Collections.Generic;
using System.Text;
using PHP.Tree;

namespace PHP.Execution
{
    public sealed class Interpreters
    {
        public static Result Execute (Expression expression, Scope scope)
        {
            if (expression == null)
            {
                return Result.NULL;
            }

            switch (expression)
            {
                case FinalExpression e:
                    return new Result (e);

                case DocExpression e:
                    return Result.NULL;

                case RequireFileExpression e:
                    return RequireFileInterpreter.Run (e, scope);

                case BlockExpression e:
                    return BlockInterpreter.Run (e, scope);

                case ConditionalBlockExpression e:
                    return ConditionalBlockInterpreter.Run (e, scope);
                case TryExpression e:
                    return TryInterpreter.Run (e, scope);

                case BinaryExpression e:
                    return BinaryInterpreter.Run (e, scope);

                case FunctionCallExpression e:
                    return FunctionCallInterpreter.Run (e, scope);
                case CallParameter e:
                    return CallParameterInterpreter.Run (e, scope);

                case FunctionDeclarationExpression e:
                    return FunctionDeclarationInterpreter.Run (e, scope);

                case VariableExpression e:
                    return VariableInterpreter.Run (e, scope);
                case PseudoConstExpression e:
                    return VariableInterpreter.Run (e, scope);
                case AssignExpression e:
                    return AssignInterpreter.Run (e, scope);

                default:
                    Log.Error ($"Unable to execute expression: {expression}");
                    return Result.NULL;
            }
        }

    }
}
