using System;

namespace Assign3
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Assign3())
                game.Run();
        }
    }
}
