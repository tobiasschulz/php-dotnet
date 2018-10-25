using System;

namespace PHP.Interpreter
{
    class Program
    {
        static void Main (string [] args)
        {
            Console.WriteLine ("Hello World!");

            Context context = new Context ();
            context.AddDirectory (@"C:\tobias.schulz\GIT\dfd_posdb\src");
            context.AddDirectory (@"C:\tobias.schulz\GIT\dk_framework\src");
            context.AddDirectory (@"C:\tobias.schulz\GIT\dk_rm\src");
            context.AddDirectory (@"C:\tobias.schulz\GIT\dk_app\src");
            context.RunFile (@"C:\tobias.schulz\GIT\dk_framework\src\public\index.php");

            Console.ReadLine ();
        }
    }
}
