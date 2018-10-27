using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;

namespace PHP.Tree
{
    public sealed class FunctionSignature
    {
        public readonly ImmutableArray<FunctionParameter> Parameters;
       
        public FunctionSignature (CallSignature e)
        {
            Parameters = e.Parameters.Select (p => new FunctionParameter (p)).ToImmutableArray ();
        }
    }

    public sealed class FunctionParameter : Expression
    {
        public readonly Expression Expression;
        //
        // Summary:
        //     Gets value indicating whether the parameter is prefixed by & character.
        public bool Ampersand { get; }
        //
        // Summary:
        //     Gets value indicating whether the parameter is passed with ... prefix and so
        //     it has to be unpacked before passing to the function call.
        public bool IsUnpack { get; }

        public FunctionParameter (ActualParam p)
        {
            Expression = Expressions.Parse (p.Expression);
            Ampersand = p.Ampersand;
            IsUnpack = p.IsUnpack;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("", Expression)
            };
        }

        protected override string GetTypeName ()
        {
            return $"parameter: {0}";
        }

    }
}
