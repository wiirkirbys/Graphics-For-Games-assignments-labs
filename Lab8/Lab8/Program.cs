using System;

namespace Lab8
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lab8())
                game.Run();
        }
    }
}
