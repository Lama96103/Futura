using Futura.Engine.Rendering;
using Futura.Engine.Settings;
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
    public partial class Runtime : Singleton<Runtime>
    {
        public Context Context { get; init; }

        private TimeSystem timeSys;

        public DirectoryInfo AssetDir { get; set; } = new DirectoryInfo(@"..\..\..\Assets");

        public SettingsController Settings { get; init; }

        private List<IRuntimeCommand> runtimeCommands = new List<IRuntimeCommand>();

        public RuntimeState State { get; private set; } = RuntimeState.Editor;

        private double fixedTimeState = 0;

        public Runtime()
        {
            Context = new Context();
            Settings = new SettingsController(new DirectoryInfo(@"..\..\..\..\.\..\Settings"), "Futura.Engine.Settings");
        }


        public void Init()
        {
            Profiler.StartFrame();

            timeSys = Context.RegisterSubSystem<TimeSystem>();
            Context.RegisterSubSystem<LogSystem>();
            LogView logView = new LogView();


            Context.RegisterSubSystem<InputSystem>();
            Context.RegisterSubSystem<WorldSystem>();
            Context.RegisterSubSystem<RenderSystem>();
            Context.RegisterSubSystem<Physics.PhysicsController>(Context.TickType.Fixed);

            Context.Init();


            // Init some UI elements
            UIController.Instance.Register(new MainMenuView());
            UIController.Instance.Register(logView);
            UIController.Instance.Register(new SceneView());
            UIController.Instance.Register(new SceneHierarchyView());
            UIController.Instance.Register(new PropertyView());
            UIController.Instance.Register(new AssetView());
            UIController.Instance.Register(new PerformanceView());

            // Init singleton systems
            Resources.ResourceManager.Instance.Init(AssetDir);


            EditorSettings editorSettings = Settings.Get<EditorSettings>();
            EditorCamera.Instance.Transform.Position = editorSettings.LastEditorCameraPosition;
            EditorCamera.Instance.Transform.Rotation = editorSettings.LastEditorCameraRotation;
            EditorCamera.Instance.Settings = editorSettings;
            Profiler.EndFrame();
        }
        private Task physicsTask = null;
        public void Tick()
        {
            Profiler.StartFrame();

            if(runtimeCommands.Count > 0)
            {
                foreach(IRuntimeCommand command in runtimeCommands)
                    command.Execute(this, Context);
                runtimeCommands.Clear();
            }

            
            fixedTimeState += timeSys.DeltaTime;
            if(fixedTimeState >= Time.FixedDeltaTime)
            {
                physicsTask = ThreadController.CreateTask(() => Context.Tick(Context.TickType.Fixed, Time.FixedDeltaTime), "Physics_Task");
                fixedTimeState -= Time.FixedDeltaTime;
            }

            Context.Tick(Context.TickType.Variable, timeSys.DeltaTime);

            if (physicsTask != null) physicsTask.Wait();

            if (Window.Instance.Exists)
            {
                EditorSettings editorSettings = Settings.Get<EditorSettings>();
                editorSettings.LastEditorCameraPosition = EditorCamera.Instance.Transform.Position;
                editorSettings.LastEditorCameraRotation = EditorCamera.Instance.Transform.Rotation;
                Settings.Save(editorSettings);
            }

            Profiler.EndFrame();
        }

        public void ExecuteCommand(IRuntimeCommand command)
        {
            runtimeCommands.Add(command);
        }


        public enum RuntimeState
        {
            Editor, Playing, Pause
        }
    }
}
