using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Library.TypeSystem;

namespace PHP.Tree
{
    public sealed class DeclarationSignature
    {
        public readonly ImmutableArray<DeclarationParameter> Parameters;

        public DeclarationSignature (ImmutableArray<DeclarationParameter> parameters)
        {
            Parameters = parameters;
        }
    }

    public sealed class DeclarationParameter : Expression
    {
        public readonly NameOfVariable Name;
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

        public DeclarationParameter (NameOfVariable name, Expression initial_value, bool passed_by_ref, bool is_out, bool is_variadic)
        {
            Name = name;
            InitialValue = initial_value;
            PassedByRef = passed_by_ref;
            IsOut = is_out;
            IsVariadic = is_variadic;
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
