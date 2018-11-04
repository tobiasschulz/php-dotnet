using PHP.Library.Internal;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class BreakInterpreter
    {
        public static Result Run (BreakExpression block, Scope scope)
        {
            throw new BreakException (Interpreters.Execute (block.CountOfLoops, scope).ResultValue.GetLongValue ().Clamp (1, 1000));
        }
    }

}
