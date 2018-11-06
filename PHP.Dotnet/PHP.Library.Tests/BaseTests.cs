using System;
using System.Linq;
using PHP;
using Xunit;

public abstract class BaseTests
{

    protected string _eval (string code, Action<ContextOptions> on_options = null)
    {
        ContextOptions options = new ContextOptions ();
        on_options?.Invoke (options);
        Context context = new Context (
            options: options,
            parser: new PHP.Parser.Parser ()
        );
        return context.Eval (code);
    }

    protected string _trimLines (string value)
    {
        return string.Join ("\n", value.Trim ().Replace ("\r", "").Split ("\n").Select (l => l.Trim ())).Trim ();
    }

}
