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
                case WhileExpression e:
                    return WhileInterpreter.Run (e, scope);
                case TryExpression e:
                    return TryInterpreter.Run (e, scope);

                case BreakExpression e:
                    return BreakInterpreter.Run (e, scope);
                case ContinueExpression e:
                    return ContinueInterpreter.Run (e, scope);
                case ReturnExpression e:
                    return ReturnInterpreter.Run (e, scope);

                case BinaryExpression e:
                    return BinaryInterpreter.Run (e, scope);
                case UnaryExpression e:
                    return UnaryInterpreter.Run (e, scope);

                case CallParameter e:
                    return CallParameterInterpreter.Run (e, scope);

                case FunctionCallExpression e:
                    return FunctionInterpreter.Run (e, scope);
                case FunctionDeclarationExpression e:
                    return FunctionInterpreter.Run (e, scope);

                case MethodCallExpression e:
                    return ClassInterpreter.Run (e, scope);
                case StaticMethodCallExpression e:
                    return ClassInterpreter.Run (e, scope);
                case ClassDeclarationExpression e:
                    return ClassInterpreter.Run (e, scope);
                case NewInstanceExpression e:
                    return ClassInterpreter.Run (e, scope);

                case ArrayCreateExpression e:
                    return ArrayInterpreter.Run (e, scope);
                case ArrayAccessExpression e:
                    return ArrayInterpreter.Run (e, scope);

                case VariableExpression e:
                    return VariableInterpreter.Run (e, scope);
                case PseudoConstExpression e:
                    return VariableInterpreter.Run (e, scope);
                case AssignExpression e:
                    return AssignInterpreter.Run (e, scope);
                case UnsetExpression e:
                    return AssignInterpreter.Run (e, scope);

                default:
                    Log.Error ($"Unable to execute expression: {expression}");
                    return Result.NULL;
            }
        }

    }
}
