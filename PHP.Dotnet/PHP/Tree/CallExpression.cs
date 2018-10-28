using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;
using PHP.Standard;

namespace PHP.Tree
{
    public abstract class CallExpression : Expression
    {
        public readonly CallSignature Signature;

        protected CallExpression (CallSignature signature)
        {
            Signature = signature;
        }
    }

    public sealed class FunctionCallExpression : CallExpression
    {
        public readonly Name Name;
        public readonly Expression MemberOf;

        public FunctionCallExpression (DirectFcnCall e)
            : base (new CallSignature (e.CallSignature))
        {
            Name = e.FullName;
            MemberOf = Expressions.Parse (e.IsMemberOf);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("member of", MemberOf),
                ("parameters", Signature.Parameters),
            };
        }

        protected override string GetTypeName ()
        {
            return $"function call: {Name}";
        }
    }

    public sealed class StaticMethodCallExpression : CallExpression
    {
        public readonly Name Name;
        public readonly Expression MemberOf;

        public StaticMethodCallExpression (DirectStMtdCall e)
            : base (new CallSignature (e.CallSignature))
        {
            Name = e.MethodName;
            MemberOf = Expressions.Parse (e.IsMemberOf);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("member of", MemberOf),
                ("parameters", Signature.Parameters),
            };
        }

        protected override string GetTypeName ()
        {
            return $"static method call: {Name}";
        }
    }

    public sealed class NewInstanceExpression : CallExpression
    {
        public readonly Name? TypeName;

        public NewInstanceExpression (NewEx e)
            : base (new CallSignature (e.CallSignature))
        {
            TypeName = e.ClassNameRef.QualifiedName;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("parameters", Signature.Parameters),
            };
        }

        protected override string GetTypeName ()
        {
            return $"new: {TypeName}";
        }
    }

    public sealed class EchoExpression : Expression
    {
        public readonly ImmutableArray<Expression> Parameters;

        public EchoExpression (EchoStmt e)
        {
            Parameters = e.Parameters.Select (c => Expressions.Parse (c)).ToImmutableArray ();
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("parameters", Parameters),
            };
        }

        protected override string GetTypeName ()
        {
            return $"echo";
        }
    }

    public sealed class DieExpression : Expression
    {
        public readonly Expression Result;

        public DieExpression (ExitEx e)
        {
            Result = Expressions.Parse (e.ResulExpr);
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("result", Result),
            };
        }

        protected override string GetTypeName ()
        {
            return $"echo";
        }
    }

}
