using System;
using PHP;
using Xunit;

public abstract class BaseTests
{

    protected string _eval (string code)
    {
        Context context = new Context ();
        return context.Eval (code);
    }

}
