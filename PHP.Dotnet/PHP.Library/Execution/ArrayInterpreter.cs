using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using PHP.Library.Internal;
using PHP.Library.TypeSystem;
using PHP.Standard;
using PHP.Tree;

namespace PHP.Execution
{
    public static class ArrayInterpreter
    {
        public static Result Run (ArrayCreateExpression expression, Scope scope)
        {
            IArray arr = new ArrayStructure ();
            scope.Root.Arrays.Add (arr);

            foreach (ArrayItemExpression item_expr in expression.Items)
            {
                FinalExpression key_expr = Interpreters.Execute (item_expr.Key, scope).ResultValue;
                FinalExpression value_expr = Interpreters.Execute (item_expr.Value, scope).ResultValue;
                ArrayKey key = new ArrayKey (key_expr.GetStringValue ());
                arr.Set (new ArrayItem (key, value_expr));
            }

            return new Result (arr.AsExpression);
        }

        public static Result Run (ArrayAccessExpression expression, Scope scope)
        {
            Result res = Result.NULL;
            Resolve (expression, scope, (arr, key) =>
            {
                if (arr.TryGetValue (key, out ArrayItem item))
                {
                    res = new Result (item.Value);
                }
                else
                {
                    Log.Debug ($"Undefined index: {key}, array: {arr}, scope: {scope}");
                    Log.Error ($"  existing indexes: {arr.GetAll ().Select (f => f.Key).Join (", ")}");
                }
            });
            return res;
        }

        public static void Resolve (ArrayAccessExpression expression, Scope scope, Action<IArray, ArrayKey> action)
        {
            FinalExpression key_expr = Interpreters.Execute (expression.Index, scope).ResultValue;
            ArrayKey key = new ArrayKey (key_expr.GetStringValue ());

            FinalExpression array_expr = Interpreters.Execute (expression.Array, scope).ResultValue;
            if (array_expr is ArrayPointerExpression pointer)
            {
                if (pointer.Array is IArray arr)
                {
                    action (arr, key);
                }
                else
                {
                    Log.Error ($"Array could not be found: {pointer.Array.Name}, key: '{key}', scope: {scope}");
                }
            }
            else
            {
                Log.Error ($"Array access of key '{key}' is not performed on an array, but on: {array_expr}, scope: {scope}");
            }
        }


    }

}
