using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Library.TypeSystem;
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
        public readonly NameOfFunction Name;

        public FunctionCallExpression (NameOfFunction name, CallSignature call_signature)
            : base (call_signature)
        {
            Name = name;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("parameters", CallSignature.Parameters),
            };
        }

        protected override string GetTypeName ()
        {
            return $"function call: {Name}";
        }
    }

    public class MethodCallExpression : CallExpression
    {
        public readonly NameOfMethod Name;
        public readonly Expression MemberOf;

        public MethodCallExpression (NameOfMethod name, Expression member_of, CallSignature call_signature)
            : base (call_signature)
        {
            Name = name;
            MemberOf = member_of;
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
            return $"method call: {Name}";
        }
    }

    public sealed class StaticMethodCallExpression : CallExpression
    {
        public readonly NameOfMethod Name;
        public readonly NameOfClass TargetType;

        public StaticMethodCallExpression (NameOfMethod name, NameOfClass target_type, CallSignature call_signature)
            : base (call_signature)
        {
            Name = name;
            TargetType = target_type;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("parameters", CallSignature.Parameters),
            };
        }

        protected override string GetTypeName ()
        {
            return $"static method call: {TargetType}::{Name}";
        }
    }

    public sealed class NewInstanceExpression : CallExpression
    {
        public readonly NameOfClass Name;

        public NewInstanceExpression (NameOfClass name, CallSignature call_signature)
            : base (call_signature)
        {
            Name = name;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("parameters", CallSignature.Parameters),
            };
        }

        protected override string GetTypeName ()
        {
            return $"new: {Name}";
        }
    }

    public sealed class EchoExpression : FunctionCallExpression
    {
        public EchoExpression (CallSignature call_signature)
            : base ("echo", call_signature)
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

        public DieExpression (CallSignature call_signature)
            : base ("die", call_signature)
        {
        }

        protected override string GetTypeName ()
        {
            return $"die";
        }
    }

    public sealed class IssetExpression : FunctionCallExpression
    {
        public IssetExpression (CallSignature call_signature)
            : base ("isset", call_signature)
        {
        }

        protected override string GetTypeName ()
        {
            return $"isset";
        }
    }

    public sealed class UnsetExpression : Expression
    {
        public readonly ImmutableArray<Expression> Variables;

        public UnsetExpression (ImmutableArray<Expression> variables)
        {
            Variables = variables;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("variables", Variables),
            };
        }

        protected override string GetTypeName ()
        {
            return $"unset";
        }
    }

}
