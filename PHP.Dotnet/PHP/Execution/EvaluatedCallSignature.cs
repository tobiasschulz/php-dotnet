using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Execution;
using PHP.Tree;

namespace PHP.Execution
{
    public sealed class EvaluatedCallSignature
    {
        public readonly ImmutableArray<EvaluatedCallParameter> Parameters;

        public EvaluatedCallSignature (CallSignature call_signature, Scope scope)
        {
            Parameters = call_signature.Parameters.Select (p => new EvaluatedCallParameter (p, scope)).ToImmutableArray ();
        }

    }

    public sealed class EvaluatedCallParameter
    {
        public readonly FinalExpression EvaluatedValue;

        //
        // Summary:
        //     Gets value indicating whether the parameter is prefixed by & character.
        public bool Ampersand { get; }
        //
        // Summary:
        //     Gets value indicating whether the parameter is passed with ... prefix and so
        //     it has to be unpacked before passing to the function call.
        public bool IsUnpack { get; }

        public EvaluatedCallParameter (CallParameter p, Scope scope)
        {
            EvaluatedValue = Interpreters.Execute (p, scope).ResultValue;
            Ampersand = p.Ampersand;
            IsUnpack = p.IsUnpack;
        }

    }
}
