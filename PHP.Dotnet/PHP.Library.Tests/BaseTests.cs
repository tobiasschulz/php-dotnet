using System;
using PHP;
using Xunit;

public abstract class BaseTests
{

    protected string _eval (string code, Action<ContextOptions> on_options = null)
    {
        ContextOptions options = new ContextOptions ();
        on_options?.Invoke (options);
        Context context = new Context (options);
        return context.Eval (code);
    }

}
