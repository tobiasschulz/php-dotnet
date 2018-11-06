using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Library.Internal;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class ForeachInterpreter
    {
        public static Result Run (ForeachExpression block, Scope scope)
        {
            Result overall_result = Result.NULL;

            int i = 0;
            while (true)
            {
                bool condition_is_true;
                if (block.IsDoWhile && i == 0)
                {
                    condition_is_true = true;
                }
                else
                {
                    // Log.Debug (block.Condition);
                    Result condition_result = Interpreters.Execute (block.Condition, scope);
                    condition_is_true = condition_result.IsTrue ();
                }

                if (condition_is_true)
                {
                    try
                    {
                        Result body_result = Interpreters.Execute (block.Body, scope);
                        overall_result = body_result;
                    }
                    catch (BreakException ex)
                    {
                        // Log.Debug (ex);
                        if (ex.CountOfLoops > 1)
                        {
                            throw new BreakException (ex.CountOfLoops - 1);
                        }
                        break;
                    }
                    catch (ContinueException ex)
                    {
                        // Log.Debug (ex);
                        if (ex.CountOfLoops > 1)
                        {
                            throw new ContinueException (ex.CountOfLoops - 1);
                        }
                    }
                }
                else
                {
                    break;
                }

                i++;
            }

            return overall_result;
        }
    }


}
