using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Devsense.PHP.Syntax;
using Devsense.PHP.Syntax.Ast;
using PHP.Standard;

namespace PHP.Parser
{
    public sealed class SyntaxTree : IParseResult
    {
        readonly CodeSourceUnit _source;

        public Tree.Expression RootExpression { get; private set; }

        public ImmutableArray<IDiagnostic> Diagnostics { get; private set; }

        /// <summary>
        /// Gets constructed lambda nodes.
        /// </summary>
        public ImmutableArray<LambdaFunctionExpr> Lambdas { get; private set; }

        /// <summary>
        /// Gets constructed type declaration nodes.
        /// </summary>
        public ImmutableArray<TypeDecl> Types { get; private set; }

        /// <summary>
        /// Gets constructed function declaration nodes.
        /// </summary>
        public ImmutableArray<FunctionDecl> Functions { get; private set; }

        /// <summary>
        /// Gets constructed global code (ast root).
        /// </summary>
        public GlobalCode Root { get; private set; }

        /// <summary>
        /// Gets constructed yield extpressions.
        /// </summary>
        public ImmutableArray<LangElement> YieldNodes { get; private set; }

        private SyntaxTree (CodeSourceUnit source)
        {
            _source = source ?? throw new ArgumentNullException (nameof (source));
        }

        public static SyntaxTree ParseCode (Parser parser, Context context, string content_string, string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException (nameof (filename));
            }

            content_string = content_string.Replace ("\r", "").Replace ("else if", "elseif").Trim ();

            // TODO: new parser implementation based on Roslyn

            // TODO: file.IsScript ? scriptParseOptions : parseOptions
            var unit = new CodeSourceUnit (
                content_string,
                filename,
                Encoding.UTF8,
                Lexer.LexicalStates.INITIAL,
                LanguageFeatures.Php73Set | LanguageFeatures.ShortOpenTags
            );

            var result = new SyntaxTree (unit);

            var errorSink = new PhpErrorSink (result);
            var factory = new NodesFactory (unit, context.Defines);

            //
            unit.Parse (factory, errorSink);

            //
            result.Diagnostics = errorSink.Diagnostics.Select (d => (IDiagnostic)d).ToImmutableArray ();

            result.Lambdas = factory.Lambdas.AsImmutableSafe ();
            result.Types = factory.Types.AsImmutableSafe ();
            result.Functions = factory.Functions.AsImmutableSafe ();
            result.YieldNodes = factory.YieldNodes.AsImmutableSafe ();

            if (factory.Root != null)
            {
                result.Root = factory.Root;
            }
            else
            {
                // Parser leaves factory.Root to null in the case of syntax errors -> create a proxy syntax node
                var fullSpan = new Devsense.PHP.Text.Span (0, unit.Code.Length);
                result.Root = new GlobalCode (fullSpan, ImmutableArray<Statement>.Empty, unit);
            }

            result.RootExpression = parser.Parse (result.Root);

            return result;
        }


    }
}
