using System;

namespace Assign2
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Assign2())
                game.Run();
        }
    }
}
