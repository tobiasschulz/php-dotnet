using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using PHP.Library.TypeSystem;
using PHP.Standard;
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
                    return new BoolExpression (e.Value);
                case StringLiteral e:
                    return new StringExpression (e.Value ?? string.Empty);
                case LongIntLiteral e:
                    return new LongExpression (e.Value);
                case DoubleLiteral e:
                    return new DoubleExpression (e.Value);

                case BlockStmt e:
                    return ToBlockExpression (e);
                case IfStmt e:
                    return ToConditionalBlockExpression (e);
                case TryStmt e:
                    return ToTryExpression (e);
                case ForeachStmt e:
                    return ToForeachExpression (e);
                case WhileStmt e:
                    return ToWhileExpression (e);
                case ConditionalEx e:
                    return ToConditionalBlockExpression (e);

                case DirectStMtdCall e:
                    return ToStaticMethodCallExpression (e);
                case DirectFcnCall e when e.IsMemberOf != null:
                    return ToMethodCallExpression (e);
                case DirectFcnCall e when e.IsMemberOf == null:
                    return ToFunctionCallExpression (e);

                case ArrayEx e:
                    return ToArrayExpression (e);
                case ItemUse e:
                    return ToArrayAccessExpression (e);

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

                case UnsetStmt e:
                    return ToUnsetExpression (e);

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
                case BinaryEx e when e.Operation == Operations.Identical:
                    return ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.EQUAL);
                case BinaryEx e when e.Operation == Operations.And:
                    return ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.AND);
                case BinaryEx e when e.Operation == Operations.Or:
                    return ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.OR);
                case BinaryEx e when e.Operation == Operations.NotEqual:
                    return new NotExpression (ToBinaryExpression (e.LeftExpr, e.RightExpr, BinaryOp.EQUAL));
                case BinaryEx e when e.Operation == Operations.NotIdentical:
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
                case UnaryEx e when e.Operation == Operations.BoolCast:
                    return ToUnaryExpression (e.Expr, UnaryOp.CAST_BOOL);
                case UnaryEx e when e.Operation == Operations.ArrayCast:
                    return ToUnaryExpression (e.Expr, UnaryOp.CAST_ARRAY);
                case UnaryEx e when e.Operation == Operations.ObjectCast:
                    return ToUnaryExpression (e.Expr, UnaryOp.CAST_OBJECT);
                case UnaryEx e when e.Operation == Operations.LogicNegation:
                    return ToUnaryExpression (e.Expr, UnaryOp.LOGICAL_NEGATION);
                case UnaryEx e when e.Operation == Operations.AtSign:
                    return ToUnaryExpression (e.Expr, UnaryOp.AT_SIGN);

                case EmptyEx e:
                    return ToUnaryExpression (e.Expression, UnaryOp.IS_EMPTY);

                case ConcatEx e:
                    return ToBinaryExpression (e);

                case Devsense.PHP.Syntax.Ast.Expression e:
                    Log.Error ($"Expression: {e}, {e.Operation}");
                    return new EmptyExpression ();

                default:
                    Log.Error ($"Unknown language element: {element}");
                    return new EmptyExpression ();
            }
        }

        private static ArrayCreateExpression ToArrayExpression (ArrayEx e)
        {
            ImmutableArray<ArrayItemExpression> items = e.Items.Select (i =>
            {
                if (i == null) return null;
                switch (i)
                {
                    case ValueItem v:
                        return (ArrayItemExpression)ToArrayItemValueExpression (v);
                    case RefItem v:
                        return (ArrayItemExpression)ToArrayItemRefExpression (v);
                    default:
                        Log.Error ($"Unknown kind of array item: {i}");
                        return null;
                }
            }).Where (i => i != null).ToImmutableArray ();

            return new ArrayCreateExpression (items);
        }

        private static ArrayAccessExpression ToArrayAccessExpression (ItemUse e)
        {
            return new ArrayAccessExpression (
                array: Parse (e.Array),
                index: Parse (e.Index)
            );
        }

        private static ArrayItemExpression ToArrayItemRefExpression (RefItem e)
        {
            return new ArrayItemRefExpression (
                key: Parse (e.Index),
                value: Parse (e.RefToGet)
            );
        }

        private static ArrayItemExpression ToArrayItemValueExpression (ValueItem e)
        {
            return new ArrayItemValueExpression (
                key: Parse (e.Index),
                value: Parse (e.ValueExpr)
            );
        }

        private static ConditionalBlockExpression ToConditionalBlockExpression (ConditionalEx e)
        {
            return new ConditionalBlockExpression (
                new BaseIfExpression [] { new IfExpression (condition: Parse (e.CondExpr), body: Parse (e.TrueExpr)) }.ToImmutableArray (),
                new ElseExpression (Parse (e.FalseExpr))
            );
        }

        private static ConditionalBlockExpression ToConditionalBlockExpression (IfStmt e)
        {
            ImmutableArray<BaseIfExpression> if_expr = ImmutableArray<BaseIfExpression>.Empty;
            ElseExpression else_expr = null;
            foreach (var c in e.Conditions)
            {
                if (c.Condition == null)
                {
                    if (else_expr == null)
                    {
                        else_expr = ToElseExpression (c);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException ($"Multiple else expressions ???");
                    }
                }
                else
                {
                    if (if_expr.Length == 0)
                    {
                        if_expr = if_expr.Add (ToIfExpression (c));
                    }
                    else
                    {
                        if_expr = if_expr.Add (ToElseIfExpression (c));
                    }
                }
            }

            return new ConditionalBlockExpression (
                if_expr,
                else_expr
            );
        }

        private static IfExpression ToIfExpression (ConditionalStmt c)
        {
            return new IfExpression (condition: Parse (c.Condition), body: Parse (c.Statement));
        }

        private static ElseIfExpression ToElseIfExpression (ConditionalStmt c)
        {
            return new ElseIfExpression (condition: Parse (c.Condition), body: Parse (c.Statement));
        }

        private static ElseExpression ToElseExpression (ConditionalStmt c)
        {
            return new ElseExpression (body: Parse (c.Statement));
        }

        private static WhileExpression ToWhileExpression (WhileStmt e)
        {
            return new WhileExpression (
                condition: Parse (e.CondExpr),
                body: Parse (e.Body),
                is_do_while: e.LoopType == WhileStmt.Type.Do
            );
        }

        private static ForeachExpression ToForeachExpression (ForeachStmt e)
        {
            return new ForeachExpression (
                enumeree: Parse (e.Enumeree),
                body: Parse (e.Body),
                key_variable: Parse ((VarLikeConstructUse)e.KeyVariable?.Variable ?? e.KeyVariable?.List),
                value_variable: Parse ((VarLikeConstructUse)e.ValueVariable?.Variable ?? e.ValueVariable?.List)
            );
        }

        private static TryExpression ToTryExpression (TryStmt e)
        {
            return new TryExpression (
                Parse (e.Body),
                e.Catches.Select (c => ToCatchExpression (c)).ToImmutableArray (),
                Parse (e.FinallyItem?.Body)
            );
        }

        private static CatchExpression ToCatchExpression (CatchItem e)
        {
            return new CatchExpression (
                Parse (e.Body),
                ToVariableExpression (e.Variable),
                ToNameOfClass (e.TargetType)
            );
        }

        private static RequireFileExpression ToRequireFileExpression (IncludingEx e)
        {
            return new RequireFileExpression ((RequireFileExpression.InclusionType)e.InclusionType, Parse (e.Target));
        }

        private static Expression ToInstanceOfExpression (InstanceOfEx e)
        {
            return new InstanceOfExpression (ToNameOfClass (e.ClassNameRef), Parse (e.Expression));
        }

        private static AssignExpression ToAssignExpression (ValueAssignEx e)
        {
            return new AssignExpression (Parse (e.LValue), Parse (e.RValue));
        }

        private static StaticMethodCallExpression ToStaticMethodCallExpression (DirectStMtdCall e)
        {
            // Log.Debug (e.TargetType.QualifiedName.ToJson());
            return new StaticMethodCallExpression (ToNameOfMethod (e.MethodName), ToNameOfClass (e.TargetType), ToCallSignature (e.CallSignature));
        }

        private static MethodCallExpression ToMethodCallExpression (DirectFcnCall e)
        {
            return new MethodCallExpression (ToNameOfMethod (e.FullName.OriginalName), Parse (e.IsMemberOf), ToCallSignature (e.CallSignature));
        }

        private static FunctionCallExpression ToFunctionCallExpression (DirectFcnCall e)
        {
            return new FunctionCallExpression (ToNameOfFunction (e.FullName.OriginalName), ToCallSignature (e.CallSignature));
        }

        private static NewInstanceExpression ToNewInstanceExpression (NewEx e)
        {
            return new NewInstanceExpression (ToNameOfClass (e.ClassNameRef), ToCallSignature (e.CallSignature));
        }

        private static DieExpression ToDieExpression (ExitEx e)
        {
            return new DieExpression (ToCallSignature (new CallParameter (Parse (e.ResulExpr), ampersand: false, is_unpack: false)));
        }

        private static EchoExpression ToEchoExpression (EchoStmt e)
        {
            return new EchoExpression (ToCallSignature (e.Parameters.Select (c => new CallParameter (Parse (c), ampersand: false, is_unpack: false))));
        }

        private static IssetExpression ToIssetExpression (IssetEx e)
        {
            return new IssetExpression (ToCallSignature (e.VarList.Select (c => new CallParameter (Parse (c), ampersand: false, is_unpack: false))));
        }

        private static UnsetExpression ToUnsetExpression (UnsetStmt e)
        {
            return new UnsetExpression (e.VarList.Select (c => Parse (c)).ToImmutableArray ());
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
                Parse (p.Expression),
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
            return new BreakExpression (Parse (e.Expression));
        }

        private static ContinueExpression ToContinueExpression (JumpStmt e)
        {
            return new ContinueExpression (Parse (e.Expression));
        }

        private static ReturnExpression ToReturnExpression (JumpStmt e)
        {
            return new ReturnExpression (Parse (e.Expression));
        }

        private static Expression ToBinaryExpression (ConcatEx e)
        {
            if (e.Expressions.Length == 0)
            {
                return new EmptyExpression ();
            }
            else if (e.Expressions.Length == 1)
            {
                return Parse (e.Expressions [0]);
            }
            else
            {
                Expression result_ex = ToBinaryExpression (e.Expressions [0], e.Expressions [1], BinaryOp.CONCAT);
                foreach (var additional_ex in e.Expressions.Skip (2))
                    result_ex = ToBinaryExpression (result_ex, Parse (additional_ex), BinaryOp.CONCAT);
                return result_ex;
            }
        }

        private static BinaryExpression ToBinaryExpression (Devsense.PHP.Syntax.Ast.Expression left, Devsense.PHP.Syntax.Ast.Expression right, BinaryOp operation)
        {
            return new BinaryExpression (
                Parse (left),
                Parse (right),
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
                Parse (value),
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

        private static ClassDeclarationExpression ToClassDeclarationExpression (NamedTypeDecl e)
        {
            List<ClassFieldDeclarationExpression> fields = new List<ClassFieldDeclarationExpression> ();
            List<ClassMethodDeclarationExpression> methods = new List<ClassMethodDeclarationExpression> ();

            foreach (TypeMemberDecl member in e.Members)
            {
                MemberAttributes attributes = ToMemberAttributes (member.Modifiers);
                switch (member)
                {
                    case FieldDeclList l:
                        fields.AddRange (l.Fields.Select (f => ToClassFieldDeclarationExpression (attributes, f)));
                        break;
                    case MethodDecl l:
                        methods.Add (ToClassMethodDeclarationExpression (attributes, l));
                        break;
                    default:
                        Log.Error ($"Unknown class member: {member}");
                        break;
                }
            }

            List<NameOfClass> base_classes = new List<NameOfClass> ();
            if (e.BaseClass != null)
            {
                base_classes.Add (ToNameOfClass (e.BaseClass.ClassName));
            }

            List<NameOfClass> base_interfaces = new List<NameOfClass> ();
            base_interfaces.AddRange (e.ImplementsList.Select (o => ToNameOfClass (o.ClassName)));

            return new ClassDeclarationExpression (
                ToNameOfClass (e.Name),
                base_classes.ToImmutableArray (),
                base_interfaces.ToImmutableArray (),
                fields.ToImmutableArray (),
                methods.ToImmutableArray ()
            );
        }

        private static ClassMethodDeclarationExpression ToClassMethodDeclarationExpression (MemberAttributes attributes, MethodDecl e)
        {
            return new ClassMethodDeclarationExpression (
                ToNameOfMethod (e.Name),
                ToDeclarationSignature (e.Signature),
                Parse (e.Body),
                attributes
            );
        }

        private static ClassFieldDeclarationExpression ToClassFieldDeclarationExpression (MemberAttributes attributes, FieldDecl e)
        {
            return new ClassFieldDeclarationExpression (
                ToNameOfVariable (e.Name),
                Parse (e.Initializer),
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
                Parse (p.InitValue),
                p.PassedByRef,
                p.IsOut,
                p.IsVariadic
            );
        }

        private static GlobalStmtExpression ToGlobalStmtExpression (GlobalStmt e)
        {
            return new GlobalStmtExpression (e.VarList.Select (c => Parse (c)).ToImmutableArray ());
        }

        private static BlockExpression ToBlockExpression (GlobalCode e)
        {
            return new BlockExpression (e.Statements.Select (c => Parse (c)).ToImmutableArray ());
        }

        private static BlockExpression ToBlockExpression (BlockStmt e)
        {
            return new BlockExpression (e.Statements.Select (c => Parse (c)).ToImmutableArray ());
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

        private static NameOfMethod ToNameOfMethod (QualifiedName e)
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

        private static NameOfClass ToNameOfClass (QualifiedName e)
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

    }
}
