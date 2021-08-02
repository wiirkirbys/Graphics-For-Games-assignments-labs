using System;

namespace Lab7
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lab7())
                game.Run();
        }
    }
}
