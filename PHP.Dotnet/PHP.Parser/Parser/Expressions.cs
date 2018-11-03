using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using PHP.Library.TypeSystem;
using PHP.Tree;
using Expression = PHP.Tree.Expression;

namespace PHP.Parser
{
    internal class Expressions
    {
        internal static Expression Parse (LangElement element)
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
                    return ToBlockExpression (e);

                case GlobalStmt e:
                    return ToGlobalStmtExpression (e);

                case FunctionDecl e:
                    return ToFunctionDeclarationExpression (e);
                case NamedTypeDecl e:
                    return ToClassDeclarationExpression (e);

                case JumpStmt e when e.Type == JumpStmt.Types.Break:
                    return ToBreakExpression (e);
                case JumpStmt e when e.Type == JumpStmt.Types.Continue:
                    return ToContinueExpression (e);
                case JumpStmt e when e.Type == JumpStmt.Types.Return:
                    return ToReturnExpression (e);

                case DirectVarUse e:
                    return ToVariableExpression (e);
                case GlobalConstUse e:
                    return ToVariableExpression (e);
                case PseudoConstUse e:
                    return ToPseudoConstExpression (e);

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
                    return ToBlockExpression (e);

                case ForeachStmt e:
                    return new ForeachExpression (e);

                case DirectStMtdCall e:
                    return ToStaticMethodCallExpression (e);
                case DirectFcnCall e:
                    return ToFunctionCallExpression (e);

                case ArrayEx e:
                    return new ArrayExpression (e);

                case ValueAssignEx e:
                    return ToAssignExpression (e);

                case IncludingEx e:
                    return ToRequireFileExpression (e);

                case PHPDocStmt e:
                    return new DocExpression ();

                case InstanceOfEx e:
                    return ToInstanceOfExpression (e);

                case IssetEx e:
                    return ToIssetExpression (e);

                case EchoStmt e:
                    return ToEchoExpression (e);

                case ExitEx e:
                    return ToDieExpression (e);

                case NewEx e:
                    return ToNewInstanceExpression (e);

                case BinaryEx e when e.Operation == Operations.Mul:
                    return ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.MUL);
                case BinaryEx e when e.Operation == Operations.Div:
                    return ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.DIV);
                case BinaryEx e when e.Operation == Operations.Add:
                    return ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.ADD);
                case BinaryEx e when e.Operation == Operations.Sub:
                    return ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.SUB);
                case BinaryEx e when e.Operation == Operations.Concat:
                    return ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.CONCAT);
                case BinaryEx e when e.Operation == Operations.Equal:
                    return ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.EQUAL);
                case BinaryEx e when e.Operation == Operations.And:
                    return ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.AND);
                case BinaryEx e when e.Operation == Operations.Or:
                    return ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.OR);
                case BinaryEx e when e.Operation == Operations.NotEqual:
                    return new NotExpression (ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.EQUAL));
                case BinaryEx e when e.Operation == Operations.LessThan:
                    return ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.LESS_THAN);
                case BinaryEx e when e.Operation == Operations.LessThanOrEqual:
                    return ToBinaryExpression (ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.LESS_THAN), ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.EQUAL), BinaryOp.OR);
                case BinaryEx e when e.Operation == Operations.GreaterThan:
                    return ToBinaryExpression (new NotExpression (ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.LESS_THAN)), new NotExpression (ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.EQUAL)), BinaryOp.AND);
                case BinaryEx e when e.Operation == Operations.GreaterThanOrEqual:
                    return new NotExpression (ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.LESS_THAN));

                case UnaryEx e when e.Operation == Operations.StringCast:
                    return ToUnaryExpression (e.Expr, UnaryOp.CAST_STRING);
                case UnaryEx e when e.Operation == Operations.LogicNegation:
                    return ToUnaryExpression (e.Expr, UnaryOp.LOGICAL_NEGATION);

                case Devsense.PHP.Syntax.Ast.Expression e:
                    Log.Error ($"Expression: {e}, {e.Operation}");
                    return new EmptyExpression ();

                default:
                    Log.Error ($"Unknown language element: {element}");
                    return new EmptyExpression ();
            }
        }

        private static NameOfVariable ToNameOfVariable (VariableName e)
        {
            return new NameOfVariable (e.Value);
        }

        private static NameOfVariable ToNameOfVariable (QualifiedName e)
        {
            return new NameOfVariable (e.Name.Value);
        }

        private static NameOfMethod ToNameOfMethod (NameRef e)
        {
            return new NameOfMethod (e.Name.Value);
        }

        private static NameOfFunction ToNameOfFunction (NameRef e)
        {
            return new NameOfFunction (e.Name.Value);
        }

        private static NameOfFunction ToNameOfFunction (QualifiedName e)
        {
            return new NameOfFunction (e.Name.Value);
        }

        private static NameOfClass ToNameOfClass (NameRef e)
        {
            return new NameOfClass (e.Name.Value);
        }

        private static NameOfClass ToNameOfClass (TypeRef e)
        {
            if (e.QualifiedName.HasValue)
            {
                return new NameOfClass (e.QualifiedName.Value.Name.Value);
            }
            else
            {
                Log.Error ($"name of class is empty: {e}");
                return new NameOfClass ();
            }
        }

        private static RequireFileExpression ToRequireFileExpression (IncludingEx e)
        {
            return new RequireFileExpression ((RequireFileExpression.InclusionType)e.InclusionType, Expressions.Parse (e.Target));
        }

        private static Expression ToInstanceOfExpression (InstanceOfEx e)
        {
            return new InstanceOfExpression (ToNameOfClass (e.ClassNameRef), Expressions.Parse (e.Expression));
        }

        private static AssignExpression ToAssignExpression (ValueAssignEx e)
        {
            return new AssignExpression (Expressions.Parse (e.LValue), Expressions.Parse (e.RValue));
        }

        private static FunctionCallExpression ToFunctionCallExpression (DirectFcnCall e)
        {
            return new FunctionCallExpression (ToNameOfFunction (e.FullName.OriginalName), Expressions.Parse (e.IsMemberOf), ToCallSignature (e.CallSignature));
        }

        private static StaticMethodCallExpression ToStaticMethodCallExpression (DirectStMtdCall e)
        {
            return new StaticMethodCallExpression (ToNameOfMethod (e.MethodName), Expressions.Parse (e.IsMemberOf), ToCallSignature (e.CallSignature));
        }

        private static NewInstanceExpression ToNewInstanceExpression (NewEx e)
        {
            return new NewInstanceExpression (ToNameOfClass (e.ClassNameRef), ToCallSignature (e.CallSignature));
        }

        private static DieExpression ToDieExpression (ExitEx e)
        {
            return new DieExpression (ToCallSignature (new CallParameter (Expressions.Parse (e.ResulExpr), ampersand: false, is_unpack: false)));
        }

        private static EchoExpression ToEchoExpression (EchoStmt e)
        {
            return new EchoExpression (ToCallSignature (e.Parameters.Select (c => new CallParameter (Expressions.Parse (c), ampersand: false, is_unpack: false))));
        }

        private static IssetExpression ToIssetExpression (IssetEx e)
        {
            return new IssetExpression (ToCallSignature (e.VarList.Select (c => new CallParameter (Expressions.Parse (c), ampersand: false, is_unpack: false))));
        }

        private static Tree.CallSignature ToCallSignature (IEnumerable<CallParameter> e)
        {
            return new Tree.CallSignature (e);
        }

        private static Tree.CallSignature ToCallSignature (CallParameter e)
        {
            return new Tree.CallSignature (e);
        }

        private static Tree.CallSignature ToCallSignature (Devsense.PHP.Syntax.Ast.CallSignature e)
        {
            return new Tree.CallSignature (e.Parameters.Select (p => ToCallParameter (p)));
        }

        private static CallParameter ToCallParameter (ActualParam p)
        {
            return new CallParameter (
                Expressions.Parse (p.Expression),
                ampersand: p.Ampersand,
                is_unpack: p.IsUnpack
            );
        }

        private static PseudoConstExpression ToPseudoConstExpression (PseudoConstUse e)
        {
            return new PseudoConstExpression ((PseudoConstExpression.PseudoConstType)e.Type);
        }

        private static VariableExpression ToVariableExpression (DirectVarUse e)
        {
            return new VariableExpression (ToNameOfVariable (e.VarName));
        }

        private static VariableExpression ToVariableExpression (GlobalConstUse e)
        {
            return new VariableExpression (ToNameOfVariable (e.Name));
        }

        private static BreakExpression ToBreakExpression (JumpStmt e)
        {
            return new BreakExpression (Expressions.Parse (e.Expression));
        }

        private static ContinueExpression ToContinueExpression (JumpStmt e)
        {
            return new ContinueExpression (Expressions.Parse (e.Expression));
        }

        private static ReturnExpression ToReturnExpression (JumpStmt e)
        {
            return new ReturnExpression (Expressions.Parse (e.Expression));
        }

        private static BinaryExpression ToBinaryExpression (Devsense.PHP.Syntax.Ast.Expression left, Devsense.PHP.Syntax.Ast.Expression right, BinaryOp operation)
        {
            return new BinaryExpression (
                Expressions.Parse (left),
                Expressions.Parse (right),
                operation
            );
        }

        private static BinaryExpression ToBinaryExpression (Expression left, Expression right, BinaryOp operation)
        {
            return new BinaryExpression (
                left,
                right,
                operation
            );
        }

        private static UnaryExpression ToUnaryExpression (Devsense.PHP.Syntax.Ast.Expression value, UnaryOp operation)
        {
            return new UnaryExpression (
                Expressions.Parse (value),
                operation
            );
        }

        private static UnaryExpression ToUnaryExpression (Expression value, UnaryOp operation)
        {
            return new UnaryExpression (
                value,
                operation
            );
        }

        private static MemberAttributes ToMemberAttributes (Devsense.PHP.Syntax.PhpMemberAttributes modifiers)
        {
            Publicity publicity;
            if ((modifiers & Devsense.PHP.Syntax.PhpMemberAttributes.Public) != 0)
                publicity = Publicity.Public;
            else if ((modifiers & Devsense.PHP.Syntax.PhpMemberAttributes.Protected) != 0)
                publicity = Publicity.Protected;
            else if ((modifiers & Devsense.PHP.Syntax.PhpMemberAttributes.Private) != 0)
                publicity = Publicity.Private;
            else
                publicity = Publicity.Public;

            bool is_static = ((modifiers & Devsense.PHP.Syntax.PhpMemberAttributes.Static) != 0);
            bool is_abstract = ((modifiers & Devsense.PHP.Syntax.PhpMemberAttributes.Abstract) != 0);
            bool is_constructor = ((modifiers & Devsense.PHP.Syntax.PhpMemberAttributes.Constructor) != 0);

            return new MemberAttributes (publicity, is_static, is_abstract, is_constructor);
        }

        private static ClassMethodDeclarationExpression ToClassMethodDeclarationExpression (MemberAttributes attributes, MethodDecl e)
        {
            return new ClassMethodDeclarationExpression (
                ToNameOfMethod (e.Name),
                ToDeclarationSignature (e.Signature),
                Expressions.Parse (e.Body),
                attributes
            );
        }

        private static ClassDeclarationExpression ToClassDeclarationExpression (NamedTypeDecl e)
        {
            return new ClassDeclarationExpression (
                ToNameOfClass (e.Name),
                e.Members.SelectMany (member =>
                {
                    MemberAttributes attributes = ToMemberAttributes (member.Modifiers);
                    switch (member)
                    {
                        case FieldDeclList l:
                            return l.Fields.Select (f => ToClassFieldDeclarationExpression (attributes, f));

                        case MethodDecl l:
                            return new [] { ToClassMethodDeclarationExpression (attributes, l) };

                        default:
                            Log.Error ($"Unknown member: {member}");
                            return Enumerable.Empty<Expression> ();
                    }
                }).ToImmutableArray ()
            );
        }

        private static ClassFieldDeclarationExpression ToClassFieldDeclarationExpression (MemberAttributes attributes, FieldDecl e)
        {
            return new ClassFieldDeclarationExpression (
                ToNameOfVariable (e.Name),
                Expressions.Parse (e.Initializer),
                attributes
            );
        }


        private static FunctionDeclarationExpression ToFunctionDeclarationExpression (FunctionDecl e)
        {
            return new FunctionDeclarationExpression (
                ToNameOfFunction (e.Name),
                ToDeclarationSignature (e.Signature),
                Parse (e.Body)
            );
        }

        private static DeclarationSignature ToDeclarationSignature (Signature e)
        {
            return new DeclarationSignature (e.FormalParams.Select (p => ToDeclarationParameter (p)).ToImmutableArray ());
        }
        private static DeclarationParameter ToDeclarationParameter (FormalParam p)
        {
            return new DeclarationParameter (
                ToNameOfVariable (p.Name),
                Expressions.Parse (p.InitValue),
                p.PassedByRef,
                p.IsOut,
                p.IsVariadic
            );
        }

        private static GlobalStmtExpression ToGlobalStmtExpression (GlobalStmt e)
        {
            return new GlobalStmtExpression (e.VarList.Select (c => Expressions.Parse (c)).ToImmutableArray ());
        }

        private static BlockExpression ToBlockExpression (GlobalCode e)
        {
            return new BlockExpression (e.Statements.Select (c => Expressions.Parse (c)).ToImmutableArray ());
        }

        private static BlockExpression ToBlockExpression (BlockStmt e)
        {
            return new BlockExpression (e.Statements.Select (c => Expressions.Parse (c)).ToImmutableArray ());
        }
    }
}
