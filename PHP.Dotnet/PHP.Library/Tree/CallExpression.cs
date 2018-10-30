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
        public readonly CallSignature CallSignature;

        protected CallExpression (CallSignature signature)
        {
            CallSignature = signature;
        }
    }

    public class FunctionCallExpression : CallExpression
    {
        public readonly Name Name;
        public readonly Expression MemberOf;

        public FunctionCallExpression (DirectFcnCall e)
            : base (new CallSignature (e.CallSignature))
        {
            Name = e.FullName;
            MemberOf = Expressions.Parse (e.IsMemberOf);
        }

        public FunctionCallExpression (Name name, CallSignature call_signature)
            : base (call_signature)
        {
            Name = name;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("member of", MemberOf),
                ("parameters", CallSignature.Parameters),
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
                ("parameters", CallSignature.Parameters),
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
                ("parameters", CallSignature.Parameters),
            };
        }

        protected override string GetTypeName ()
        {
            return $"new: {TypeName}";
        }
    }

    public sealed class EchoExpression : FunctionCallExpression
    {
        public EchoExpression (EchoStmt e)
            : base ("echo", new CallSignature (e.Parameters.Select (c => Expressions.Parse (c))))
        {
        }

        protected override string GetTypeName ()
        {
            return $"echo";
        }
    }

    public sealed class DieExpression : FunctionCallExpression
    {
        public readonly Expression Result;

        public DieExpression (ExitEx e)
            : base ("die", new CallSignature (Expressions.Parse (e.ResulExpr)))
        {
        }

        protected override string GetTypeName ()
        {
            return $"die";
        }
    }

    public sealed class IssetExpression : FunctionCallExpression
    {
        public IssetExpression (IssetEx e)
            : base ("isset", new CallSignature (e.VarList.Select (c => Expressions.Parse (c))))
        {
        }

        protected override string GetTypeName ()
        {
            return $"isset";
        }
    }

}
