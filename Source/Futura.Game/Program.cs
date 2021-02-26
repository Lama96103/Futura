using Futura.Engine.Core;
using System;

namespace Futura.Game
{
    class Program
    {
        static void Main(string[] args)
        {
            Runtime runtime = Runtime.Instance;
            runtime.Init();

            Window mainWindow = Window.Instance;

            mainWindow.Init(100, 100, 1280, 720, "Futura", WindowState.Normal);

            while (mainWindow.Exists)
            {
                runtime.Tick();
                mainWindow.PumpEvents();
            }

        }
    }
}
