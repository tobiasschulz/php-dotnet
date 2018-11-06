using System;
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
            // core
            functions.Add (new echo ());
            functions.Add (new define ());
            functions.Add (new defined ());
            functions.Add (new isset ());
            functions.Add (new die ());
            functions.Add (new ini_get ());
            functions.Add (new ini_set ());
            functions.Add (new error_reporting ());
            functions.Add (new func_get_args ());

            // system
            functions.Add (new memory_get_peak_usage ());
            functions.Add (new memory_get_usage ());
            functions.Add (new microtime ());

            // IO
            functions.Add (new realpath ());
            functions.Add (new dirname ());
            functions.Add (new file_exists ());
            functions.Add (new is_file ());
            functions.Add (new is_dir ());
            functions.Add (new mkdir ());

            // functions/classes
            functions.Add (new function_exists ());
            functions.Add (new class_exists ());
            functions.Add (new interface_exists ());
            functions.Add (new get_class ());

            // type check
            functions.Add (new is_array ());
            functions.Add (new is_object ());
            functions.Add (new is_string ());
            functions.Add (new is_null ());

            // string
            functions.Add (new mb_substr ());
            functions.Add (new trim ());
            functions.Add (new ltrim ());
            functions.Add (new rtrim ());
            functions.Add (new strpos ());
            functions.Add (new stripos ());
            functions.Add (new strrpos ());
            functions.Add (new strripos ());
            functions.Add (new str_replace ());
            functions.Add (new str_ireplace ());

            // array
            functions.Add (new count ());
            functions.Add (new @sizeof ());
            functions.Add (new in_array ());
            functions.Add (new array_key_exists ());
            functions.Add (new array_merge ());
            functions.Add (new array_shift ());

            // network
            functions.Add (new gethostbyname ());
            functions.Add (new gethostname ());
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
