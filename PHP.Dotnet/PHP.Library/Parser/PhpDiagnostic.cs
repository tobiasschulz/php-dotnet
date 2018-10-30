using Devsense.PHP.Errors;
using Devsense.PHP.Text;

namespace PHP.Parser
{
    public class PhpDiagnostic
    {
        private readonly PhpSyntaxTree _syntaxTree;
        private readonly Span _span;
        private readonly ErrorInfo _info;
        private readonly string [] _args;

        public PhpDiagnostic (PhpSyntaxTree syntaxTree, Span span, ErrorInfo info, string [] args)
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
