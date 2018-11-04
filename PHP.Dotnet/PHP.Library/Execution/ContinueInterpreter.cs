using PHP.Library.Internal;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class ContinueInterpreter
    {
        public static Result Run (ContinueExpression block, Scope scope)
        {
            throw new ContinueException (Interpreters.Execute (block.CountOfLoops, scope).ResultValue.GetLongValue ().Clamp (1, 1000));
        }
    }

}
