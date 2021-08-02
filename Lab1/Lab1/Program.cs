using System;

namespace Lab1
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lab1())
                game.Run();
        }
    }
}
