using System;

namespace Lab12
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lab12())
                game.Run();
        }
    }
}
