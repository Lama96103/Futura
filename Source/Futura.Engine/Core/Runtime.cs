﻿using Futura.Engine.UserInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Core
{
    /// <summary>
    /// The runtime handles the current context of the engine, its subsystem and basic setup of user interface and other configuration
    /// </summary>
    public sealed class Runtime : Singleton<Runtime>
    {
        public Context Context { get; init; }

        private TimeSystem timeSys;

        public DirectoryInfo AssetDir { get; set; } = new DirectoryInfo(@"..\..\..\..\.\..\Assets");
        public DirectoryInfo SceneDir { get; set; } = new DirectoryInfo(@"..\..\..\..\.\..\Scenes");

        public SettingsController Settings { get; init; }

        private bool shouldLoadScene = false;
        private FileInfo sceneFile;

        public Runtime()
        {
            Context = new Context();
            Settings = new SettingsController(new DirectoryInfo(@"..\..\..\..\.\..\Settings"), "Futura.Engine.Settings");
        }


        public void Init()
        {
            timeSys = Context.RegisterSubSystem<TimeSystem>();
            Context.RegisterSubSystem<LogSystem>();
            LogView logView = new LogView();


            Context.RegisterSubSystem<InputSystem>();
            Context.RegisterSubSystem<WorldSystem>();
            Context.RegisterSubSystem<RenderSystem>();

            Context.Init();


            // Init some UI elements
            UIController.Instance.Register(new MainMenuView());
            UIController.Instance.Register(logView);
            UIController.Instance.Register(new SceneView());
            UIController.Instance.Register(new SceneHierarchyView());
            UIController.Instance.Register(new PropertyView());
            UIController.Instance.Register(new AssetView());

            // Init singleton systems
            Resources.ResourceManager.Instance.Init(AssetDir);
        }

        public void Tick()
        {
            Profiler.StartFrame();

            if (shouldLoadScene)
            {
                shouldLoadScene = false;
                Context.GetSubSystem<WorldSystem>().Load(sceneFile);
                RuntimeHelper.Instance.SelectedAsset = null;
                RuntimeHelper.Instance.SelectedEntity = null;
            }


            Context.Tick(Context.TickType.Variable, timeSys.DeltaTime);
            Context.Tick(Context.TickType.Smoothed, timeSys.DeltaTimeSmoothed);
            Profiler.EndFrame();
        }

        public void LoadScene(FileInfo file)
        {
            sceneFile = file;
            shouldLoadScene = true;
        }
    }
}
