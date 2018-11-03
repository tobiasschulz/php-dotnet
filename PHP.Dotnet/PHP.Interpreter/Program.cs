using System;
using System.IO;

namespace PHP.Interpreter
{
    class Program
    {
        static void Main (string [] args)
        {
            Context context = new Context (
                options: new ContextOptions
                {
                    DEBUG_EXECUTION = true,
                },
                parser: new PHP.Parser.Parser ()
            );

            if (Directory.Exists ("/Users/tobias/di" + "gi" + "tal" + "kr" + "aft/git/d" + @"k_fr" + @"am" + @"ew" + @"or" + @"k/"))
            {
                context.AddDirectory (@"/Users/tobias/di" + "gi" + "tal" + "kr" + "aft/git/df" + @"d_po" + @"sdb/src");
                context.AddDirectory (@"/Users/tobias/di" + "gi" + "tal" + "kr" + "aft/git/dk_" + @"fra" + @"mewo" + @"rk/src");
                context.AddDirectory (@"/Users/tobias/di" + "gi" + "tal" + "kr" + "aft/git/dk_" + @"r" + @"m/src");
                context.AddDirectory (@"/Users/tobias/di" + "gi" + "tal" + "kr" + "aft/git/dk_" + @"ap" + @"p/src");

                context.RunFile (@"/Users/tobias/di" + "gi" + "tal" + "kr" + "aft/git/dk_" + @"fra" + @"mewo" + @"rk/src/public/index.php");
            }
            else
            {
                context.AddDirectory (@"C:\tobias.schulz\GIT\df" + @"d_po" + @"sdb\src");
                context.AddDirectory (@"C:\tobias.schulz\GIT\dk_" + @"fra" + @"mewo" + @"rk\src");
                context.AddDirectory (@"C:\tobias.schulz\GIT\dk_" + @"r" + @"m\src");
                context.AddDirectory (@"C:\tobias.schulz\GIT\dk_" + @"ap" + @"p\src");

                context.RunFile (@"C:\tobias.schulz\GIT\d" + @"k_fr" + @"ame" + @"w" + @"ork\src\public\index.php");
            }

            Console.ReadLine ();
        }
    }
}
