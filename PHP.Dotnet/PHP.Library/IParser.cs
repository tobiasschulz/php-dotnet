using System.Collections.Immutable;
using PHP.Tree;

namespace PHP
{
    public interface IParser
    {
        Expression Parse (object root);
        IParseResult ParseCode (Context context, string content_string, string filename);
    }

    public interface IParseResult
    {
        Expression RootExpression { get; }
        ImmutableArray<IDiagnostic> Diagnostics { get; }
    }

    public interface IDiagnostic
    {
    }
}
