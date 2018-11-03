using Devsense.PHP.Errors;
using Devsense.PHP.Text;

namespace PHP.Parser
{
    public class Diagnostic : IDiagnostic
    {
        private readonly SyntaxTree _syntaxTree;
        private readonly Span _span;
        private readonly ErrorInfo _info;
        private readonly string [] _args;

        public Diagnostic (SyntaxTree syntaxTree, Span span, ErrorInfo info, string [] args)
        {
            this._syntaxTree = syntaxTree;
            this._span = span;
            this._info = info;
            this._args = args;
        }

        public override string ToString ()
        {
            return $"Diagnostic: {_info.ToString (_args)}";
        }
    }
}
