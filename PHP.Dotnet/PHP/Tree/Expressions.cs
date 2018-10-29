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
                    return new BlockExpression (e);

                case GlobalStmt e:
                    return new GlobalStmtExpression (e);

                case FunctionDecl e:
                    return new FunctionDeclarationExpression (e);

                case DirectVarUse e:
                    return new VariableExpression (e.VarName);
                case GlobalConstUse e:
                    return new VariableExpression (e.Name);
                case PseudoConstUse e:
                    return new PseudoConstExpression (e);

                case BoolLiteral e:
                    return new BoolExpression (e);
                case StringLiteral e:
                    return new StringExpression (e);
                case LongIntLiteral e:
                    return new LongExpression (e);
                case DoubleLiteral e:
                    return new DoubleExpression (e);

                case IfStmt e:
                    return new ConditionalBlockExpression (e);

                case TryStmt e:
                    return new TryExpression (e);

                case BlockStmt e:
                    return new BlockExpression (e);

                case ForeachStmt e:
                    return new ForeachExpression (e);

                case DirectStMtdCall e:
                    return new StaticMethodCallExpression (e);

                case DirectFcnCall e:
                    return new FunctionCallExpression (e);

                case ArrayEx e:
                    return new ArrayExpression (e);

                case ValueAssignEx e:
                    return new AssignExpression (e);

                case IncludingEx e:
                    return new RequireFileExpression (e);

                case PHPDocStmt e:
                    return new DocExpression ();

                case InstanceOfEx e:
                    return new InstanceOfExpression (e);

                case EchoStmt e:
                    return new EchoExpression (e);

                case ExitEx e:
                    return new DieExpression (e);

                case NewEx e:
                    return new NewInstanceExpression (e);

                case BinaryEx e when e.Operation == Operations.Mul:
                    return new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.MUL);
                case BinaryEx e when e.Operation == Operations.Div:
                    return new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.DIV);
                case BinaryEx e when e.Operation == Operations.Add:
                    return new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.ADD);
                case BinaryEx e when e.Operation == Operations.Sub:
                    return new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.SUB);
                case BinaryEx e when e.Operation == Operations.Concat:
                    return new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.CONCAT);
                case BinaryEx e when e.Operation == Operations.Equal:
                    return new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.EQUAL);
                case BinaryEx e when e.Operation == Operations.And:
                    return new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.AND);
                case BinaryEx e when e.Operation == Operations.Or:
                    return new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.OR);
                case BinaryEx e when e.Operation == Operations.NotEqual:
                    return new NotExpression (new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.EQUAL));
                case BinaryEx e when e.Operation == Operations.LessThan:
                    return new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.LESS_THAN);
                case BinaryEx e when e.Operation == Operations.LessThanOrEqual:
                    return new BinaryExpression (new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.LESS_THAN), new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.EQUAL), BinaryOp.OR);
                case BinaryEx e when e.Operation == Operations.GreaterThan:
                    return new BinaryExpression (new NotExpression (new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.LESS_THAN)), new NotExpression (new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.EQUAL)), BinaryOp.AND);
                case BinaryEx e when e.Operation == Operations.GreaterThanOrEqual:
                    return new NotExpression (new BinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.LESS_THAN));

                case UnaryEx e when e.Operation == Operations.StringCast:
                    return new UnaryExpression (e.Expr, UnaryOp.CAST_STRING);

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
