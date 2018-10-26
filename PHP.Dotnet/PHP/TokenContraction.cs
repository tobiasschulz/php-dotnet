using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using PHP.Helper;
using PHP.Standard;
using System.Collections.Immutable;

namespace PHP
{
    internal class TokenContraction
    {
        internal static BaseToken2 Contract (ImmutableArray<Token1> input)
        {
            var tree = TokenContraction_Step1.MakeTreeStep (input);
            var intermediate = TokenContraction_Step2.GroupOperators (tree);
            var output = TokenContraction_Step3.Transform (intermediate);
            return output;
        }

    }
}
