using Futura.Engine.UserInterface;
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

        public SettingsController Settings { get; init; }

        public Runtime()
        {
            Context = new Context();
            Settings = new SettingsController(new DirectoryInfo(@"..\..\..\..\.\..\Settings"), "Futura.Engine.Settings");
        }


        public void Init()
        {
            timeSys = Context.RegisterSubSystem<TimeSystem>();
            Context.RegisterSubSystem<LogSystem>();
            Context.RegisterSubSystem<InputSystem>();
            Context.RegisterSubSystem<WorldSystem>();
            Context.RegisterSubSystem<RenderSystem>();

            Context.Init();


            // Init some UI elements
            UIController.Instance.Register(new MainMenuView());
            UIController.Instance.Register(new LogView());
            UIController.Instance.Register(new SettingView(Settings, "Engine"));


            // Init singleton systems
            Resources.ResourceManager.Instance.Init(AssetDir);
        }

        public void Tick()
        {
            Profiler.StartFrame();
            Context.Tick(Context.TickType.Variable, timeSys.DeltaTime);
            Context.Tick(Context.TickType.Smoothed, timeSys.DeltaTimeSmoothed);
            Profiler.EndFrame();
        }
    }
}
