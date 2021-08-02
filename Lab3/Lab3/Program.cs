using System;

namespace Lab3
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Lab3())
                game.Run();
        }
    }
}
