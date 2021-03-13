﻿using Futura.Engine.Core;
using System;

namespace Futura.Game
{
    class Program
    {
        static void Main(string[] args)
        {
            Window mainWindow = Window.Instance;
            mainWindow.Init(100, 100, 1920 , 1080, "Futura", WindowState.Maximized);


            Runtime runtime = Runtime.Instance;
            runtime.Init();

            if(args.Length == 1)
            {
                runtime.ExecuteCommand(new Runtime.LoadSceneCommand(new System.IO.FileInfo(args[0])));
            }

            while (mainWindow.Exists)
            {
                runtime.Tick();
            }

        }
    }
}
