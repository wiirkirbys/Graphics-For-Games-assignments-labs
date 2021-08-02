using System;

namespace Lab2
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lab2())
                game.Run();
        }
    }
}
