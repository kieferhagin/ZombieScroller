using System;

namespace SideScroller
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SideScroller game = new SideScroller())
            {
                game.Run();
            }
        }
    }
#endif
}

