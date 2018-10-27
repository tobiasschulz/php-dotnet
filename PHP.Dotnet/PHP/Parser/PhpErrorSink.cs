using System.Collections.Generic;
using System.Collections.Immutable;
using Devsense.PHP.Errors;
using Devsense.PHP.Text;

namespace PHP.Parser
{
    internal class PhpErrorSink : IErrorSink<Span>
    {
        private readonly PhpSyntaxTree _syntaxTree;

        List<PhpDiagnostic> _diagnostics;

        public PhpErrorSink (PhpSyntaxTree syntaxTree)
        {
            _syntaxTree = syntaxTree;
        }

        public ImmutableArray<PhpDiagnostic> Diagnostics => _diagnostics != null ? _diagnostics.ToImmutableArray () : ImmutableArray<PhpDiagnostic>.Empty;

        public void Error (Span span, ErrorInfo info, params string [] argsOpt)
        {
            if (_diagnostics == null)
            {
                _diagnostics = new List<PhpDiagnostic> ();
            }

            _diagnostics.Add (new PhpDiagnostic (_syntaxTree, span, info, argsOpt));
        }
    }
}
