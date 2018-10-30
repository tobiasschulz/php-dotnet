using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax.Ast;

namespace PHP.Tree
{
    public sealed class DeclarationSignature
    {
        public readonly ImmutableArray<DeclarationParameter> Parameters;
       
        public DeclarationSignature (Devsense.PHP.Syntax.Ast.Signature e)
        {
            Parameters = e.FormalParams.Select (p => new DeclarationParameter (p)).ToImmutableArray ();
        }
    }

    public sealed class DeclarationParameter : Expression
    {
        public readonly VariableName Name;
        public readonly Expression InitialValue;

        //
        // Summary:
        //     Whether the parameter is &-modified.
        public bool PassedByRef { get; }
        //
        // Summary:
        //     Whether the parameter is an out-parameter. Set by applying the [Out] attribute.
        public bool IsOut { get; }
        //
        // Summary:
        //     Gets value indicating whether the parameter is variadic and so passed parameters
        //     will be packed into the array as passed as one parameter.
        public bool IsVariadic { get; }
        //
        // Summary:
        //     Initial value expression. Can be null.
        public Expression InitValue { get; }

        public DeclarationParameter (FormalParam p)
        {
            Name = p.Name;
            InitialValue = Expressions.Parse (p.InitValue);
            PassedByRef = p.PassedByRef;
            IsOut = p.IsOut;
            IsVariadic = p.IsVariadic;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("initial", InitValue)
            };
        }

        protected override string GetTypeName ()
        {
            return $"parameter: {Name}";
        }

    }
}
