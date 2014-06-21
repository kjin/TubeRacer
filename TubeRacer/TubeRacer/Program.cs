using System;

namespace TubeRacer
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TubeRacerGame game = new TubeRacerGame())
            {
                game.Run();
            }
        }
    }
#endif
}

