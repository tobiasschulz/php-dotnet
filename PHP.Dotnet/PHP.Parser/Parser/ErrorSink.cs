using System.Collections.Generic;
using System.Collections.Immutable;
using Devsense.PHP.Errors;
using Devsense.PHP.Text;

namespace PHP.Parser
{
    internal class PhpErrorSink : IErrorSink<Span>
    {
        private readonly SyntaxTree _syntaxTree;

        List<Diagnostic> _diagnostics;

        public PhpErrorSink (SyntaxTree syntaxTree)
        {
            _syntaxTree = syntaxTree;
        }

        public ImmutableArray<Diagnostic> Diagnostics => _diagnostics != null ? _diagnostics.ToImmutableArray () : ImmutableArray<Diagnostic>.Empty;

        public void Error (Span span, ErrorInfo info, params string [] argsOpt)
        {
            if (_diagnostics == null)
            {
                _diagnostics = new List<Diagnostic> ();
            }

            _diagnostics.Add (new Diagnostic (_syntaxTree, span, info, argsOpt));
        }
    }
}
