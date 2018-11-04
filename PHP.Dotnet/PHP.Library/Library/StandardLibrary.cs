﻿using System;
using System.Collections.Generic;
using System.Text;
using PHP.Library.Functions;
using PHP.Library.Constants;
using PHP.Tree;
using PHP.Library.TypeSystem;

namespace PHP.Library
{
    public static class StandardLibrary
    {
        public static void Populate (IFunctionCollection functions)
        {
            functions.Add (new echo ());
            functions.Add (new define ());
            functions.Add (new defined ());
            functions.Add (new isset ());
            functions.Add (new die ());
            functions.Add (new realpath ());
            functions.Add (new dirname ());
            functions.Add (new ini_get ());
            functions.Add (new ini_set ());
            functions.Add (new error_reporting ());
            functions.Add (new function_exists ());
            functions.Add (new file_exists ());
            functions.Add (new mb_substr ());
            functions.Add (new is_array ());
            functions.Add (new is_object ());
            functions.Add (new is_null ());
            functions.Add (new memory_get_peak_usage ());
            functions.Add (new memory_get_usage ());
            functions.Add (new microtime ());
        }

        public static void Populate (IVariableCollection variables)
        {
            variables.Add (new DIRECTORY_SEPARATOR ());
            variables.Add (new @true ());
            variables.Add (new @false ());
            variables.Add (new @null ());
        }
    }
}
