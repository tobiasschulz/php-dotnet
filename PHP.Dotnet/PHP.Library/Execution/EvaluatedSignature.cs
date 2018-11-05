using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Execution;
using PHP.Library.TypeSystem;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public sealed class EvaluatedSignature
    {
        public readonly ImmutableArray<EvaluatedParameter> Parameters;

        public EvaluatedSignature (CallSignature call_signature, DeclarationSignature decl_signature, Scope outer_scope)
        {
            EvaluatedParameter [] parameters = new EvaluatedParameter [Math.Max (call_signature.Parameters.Length, decl_signature.Parameters.Length)];

            for (int i = 0; i < parameters.Length; i++)
            {
                DeclarationParameter decl_parameter = decl_signature.Parameters.Get (i, default_value: null);
                CallParameter call_parameter = call_signature.Parameters.Get (i, default_value: null);

                Expression unevaluated_value = null;
                if (call_parameter != null && call_parameter.Value != null)
                {
                    unevaluated_value = call_parameter.Value;
                }
                else if (decl_parameter != null)
                {
                    unevaluated_value = decl_parameter.InitialValue;
                }
                FinalExpression evaluated_value = Interpreters.Execute (unevaluated_value, outer_scope).ResultValue;

                EvaluatedParameter evaluated_parameter = new EvaluatedParameter (
                    name: (decl_parameter?.Name ?? default),
                    evaluated_value: evaluated_value,
                    ampersand: (decl_parameter?.PassedByRef ?? false) || (call_parameter?.Ampersand ?? false),
                    is_unpack: (call_parameter?.IsUnpack ?? false)
                );
                parameters [i] = evaluated_parameter;
            }

            Parameters = parameters.ToImmutableArray ();
        }

    }

    public sealed class EvaluatedParameter
    {
        public readonly NameOfVariable Name;
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

        public EvaluatedParameter (NameOfVariable name, FinalExpression evaluated_value, bool ampersand, bool is_unpack)
        {
            Name = name;
            EvaluatedValue = evaluated_value;
            Ampersand = ampersand;
            IsUnpack = is_unpack;
        }

    }
}
