using PHP.Library.Internal;
using PHP.Tree;

namespace PHP.Execution
{
    public static class ReturnInterpreter
    {
        public static Result Run (ReturnExpression block, Scope scope)
        {
            throw new ReturnException (Interpreters.Execute (block.ReturnValue, scope).ResultValue);
        }
    }

}
