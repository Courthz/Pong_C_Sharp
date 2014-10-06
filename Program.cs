using System;
using System.Collections.Generic;

namespace BaseGame
{
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            PongCSharp mygame = new PongCSharp();

            mygame.Run();
        }
    }
}