using System;

namespace Lab5
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lab5())
                game.Run();
        }
    }
}
