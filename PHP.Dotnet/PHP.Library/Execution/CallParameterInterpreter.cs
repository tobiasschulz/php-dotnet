using PHP.Tree;

namespace PHP.Execution
{
    public static class CallParameterInterpreter
    {
        public static Result Run (CallParameter expression, Scope scope)
        {
            return Interpreters.Execute (expression.Value, scope);
        }
    }

}
