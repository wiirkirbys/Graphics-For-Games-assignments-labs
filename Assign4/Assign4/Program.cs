using System;

namespace Assign4
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Assign4())
                game.Run();
        }
    }
}
