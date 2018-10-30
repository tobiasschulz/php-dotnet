using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;
using PHP.Execution;

namespace PHP.Tree
{
    public sealed class CallSignature
    {
        public readonly ImmutableArray<CallParameter> Parameters;

        public CallSignature (Devsense.PHP.Syntax.Ast.CallSignature e)
        {
            Parameters = e.Parameters.Select (p => new CallParameter (p)).ToImmutableArray ();
        }

        public CallSignature (IEnumerable<Expression> expressions)
        {
            Parameters = expressions.Select (p => new CallParameter (p)).ToImmutableArray ();
        }

        public CallSignature (Expression expression)
        {
            Parameters = new [] { new CallParameter (expression) }.ToImmutableArray ();
        }
    }

    public sealed class CallParameter : Expression
    {
        public readonly Expression Value;

        //
        // Summary:
        //     Gets value indicating whether the parameter is prefixed by & character.
        public bool Ampersand { get; }
        //
        // Summary:
        //     Gets value indicating whether the parameter is passed with ... prefix and so
        //     it has to be unpacked before passing to the function call.
        public bool IsUnpack { get; }

        public CallParameter (Expression expression)
        {
            Value = expression;
        }

        public CallParameter (ActualParam p)
        {
            Value = Expressions.Parse (p.Expression);
            Ampersand = p.Ampersand;
            IsUnpack = p.IsUnpack;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("", Value)
            };
        }

        protected override string GetTypeName ()
        {
            return $"parameter";
        }

    }

}
