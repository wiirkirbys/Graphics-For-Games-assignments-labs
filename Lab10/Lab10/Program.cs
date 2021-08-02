using System;

namespace Lab10
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lab10())
                game.Run();
        }
    }
}
