using System;

namespace Lab6
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lab6())
                game.Run();
        }
    }
}
