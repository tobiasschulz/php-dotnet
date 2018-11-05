using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Execution;

namespace PHP.Tree
{
    public sealed class CallSignature
    {
        public readonly ImmutableArray<CallParameter> Parameters;

        public CallSignature (IEnumerable<CallParameter> parameters)
        {
            Parameters = parameters.ToImmutableArray ();
        }

        public CallSignature (CallParameter parameter)
        {
            Parameters = new [] { parameter }.ToImmutableArray ();
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

        public CallParameter (Expression expression, bool ampersand, bool is_unpack)
        {
            Value = expression;
            Ampersand = ampersand;
            IsUnpack = is_unpack;
        }

        protected override TreeChildGroup [] _getChildren ()
        {
            return new TreeChildGroup [] {
                ("", Value)
            };
        }

        protected override string GetTypeName ()
        {
            return $"call parameter";
        }

    }

}
