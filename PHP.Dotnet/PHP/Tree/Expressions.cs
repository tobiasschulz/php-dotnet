using Devsense.PHP.Syntax.Ast;
using PHP.Standard;

namespace PHP.Tree
{
    public static class Expressions
    {
        public static Expression Parse (LangElement element)
        {
            if (element == null)
            {
                return new EmptyExpression ();
            }

            switch (element)
            {
                case ExpressionStmt e:
                    return Parse (e.Expression);

                case GlobalCode e:
                    return new GlobalCodeExpression (e);

                case TryStmt e:
                    return new TryExpression (e);

                case GlobalStmt e:
                    return new GlobalStmtExpression (e);

                case DirectVarUse e:
                    Log.Error ($"direct var use: {e}, {e.VarName}, {e.Operation}");
                    return new Variable (e.VarName);

                case IfStmt e:
                    return new ConditionalBlockExpression (e);

                case BlockStmt e:
                    return new BlockExpression (e);

                case DirectStMtdCall e:
                    return new StaticMethodCallExpression (e);

                case DirectFcnCall e:
                    return new FunctionCallExpression (e);

                case ValueAssignEx exp:
                    return new AssignExpression (exp);

                case Devsense.PHP.Syntax.Ast.Expression e:
                    Log.Error ($"Expression: {e}, {e.Operation}");
                    return new EmptyExpression ();

                default:
                    Log.Error ($"Unknown language element: {element}");
                    return new EmptyExpression ();
            }
        }
    }
}
