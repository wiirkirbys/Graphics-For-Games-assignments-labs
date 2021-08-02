using System;

namespace Lab3
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Assign1())
                game.Run();
        }
    }
}
