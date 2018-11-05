using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using PHP.Execution;
using PHP.Library.Internal;
using PHP.Tree;

namespace PHP.Library.Functions
{
    public sealed class str_replace : _base_str_replace
    {
        public str_replace ()
            : base ("str_replace", StringComparison.Ordinal)
        {
        }
    }

    public sealed class str_ireplace : _base_str_replace
    {
        public str_ireplace ()
            : base ("str_ireplace", StringComparison.OrdinalIgnoreCase)
        {
        }
    }

    public abstract class _base_str_replace : ManagedFunction
    {
        private readonly StringComparison _comparison;

        protected _base_str_replace (string name, StringComparison comparison)
            : base (name)
        {
            _comparison = comparison;
        }

        protected override Result _execute (ImmutableArray<EvaluatedParameter> parameters, FunctionScope function_scope)
        {
            if (parameters.Length != 3 && parameters.Length != 4)
            {
                throw new WrongParameterCountException (this, expected: 1, actual: parameters.Length);
            }

            string search = parameters [0].EvaluatedValue.GetStringValue ();
            string replace = parameters [1].EvaluatedValue.GetStringValue ();
            string subject = parameters [2].EvaluatedValue.GetStringValue ();
            string count = parameters.Length >= 4 ? parameters [3].EvaluatedValue.GetStringValue () : null;

            string res = Replace (subject, search, replace, _comparison);

            return new Result (new StringExpression (res));
        }

        private static string Replace (string str, string oldValue, string newValue, StringComparison comparison)
        {
            if (str == null)
            {
                return str;
            }
            if (str.Length == 0)
            {
                return str;
            }
            if (oldValue == null)
            {
                return str;
            }
            if (oldValue.Length == 0)
            {
                return str;
            }

            StringBuilder resultStringBuilder = new StringBuilder (str.Length);
            bool isReplacementNullOrEmpty = string.IsNullOrEmpty (newValue);

            const int valueNotFound = -1;
            int foundAt;
            int startSearchFromIndex = 0;
            while ((foundAt = str.IndexOf (oldValue, startSearchFromIndex, comparison)) != valueNotFound)
            {
                int @charsUntilReplacment = foundAt - startSearchFromIndex;
                bool isNothingToAppend = @charsUntilReplacment == 0;
                if (!isNothingToAppend)
                {
                    resultStringBuilder.Append (str, startSearchFromIndex, @charsUntilReplacment);
                }

                // Process the replacement.
                if (!isReplacementNullOrEmpty)
                {
                    resultStringBuilder.Append (newValue);
                }


                // Prepare start index for the next search.
                // This needed to prevent infinite loop, otherwise method always start search 
                // from the start of the string. For example: if an oldValue == "EXAMPLE", newValue == "example"
                // and comparisonType == "any ignore case" will conquer to replacing:
                // "EXAMPLE" to "example" to "example" to "example" … infinite loop.
                startSearchFromIndex = foundAt + oldValue.Length;
                if (startSearchFromIndex == str.Length)
                {
                    // It is end of the input string: no more space for the next search.
                    // The input string ends with a value that has already been replaced. 
                    // Therefore, the string builder with the result is complete and no further action is required.
                    return resultStringBuilder.ToString ();
                }
            }

            // Append the last part to the result.
            int @charsUntilStringEnd = str.Length - startSearchFromIndex;
            resultStringBuilder.Append (str, startSearchFromIndex, @charsUntilStringEnd);

            return resultStringBuilder.ToString ();
        }
    }
}
