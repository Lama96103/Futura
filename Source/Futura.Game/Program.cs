using Futura.Engine.Core;
using System;

namespace Futura.Game
{
    class Program
    {
        static void Main(string[] args)
        {

            Window mainWindow = Window.Instance;
            mainWindow.Init(100, 100, 1280, 720, "Futura", WindowState.Normal);


            Runtime runtime = Runtime.Instance;
            runtime.Init();

            while (mainWindow.Exists)
            {
                runtime.Tick();
            }

        }
    }
}
