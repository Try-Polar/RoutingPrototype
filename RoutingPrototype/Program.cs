using System;

namespace RoutingPrototype
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new RoutingPrototype())
                game.Run();
        }
    }
} // 1, 11, 7
