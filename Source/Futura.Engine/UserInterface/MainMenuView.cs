using Futura.Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.UserInterface
{
    public class MainMenuView : View
    {
        private string windowTitle = "Futura";

        private bool isLoadSceneDialogOpen = false;
        private bool isNewSceneDialogOpen = false;

        private FileInfo selectedFile = null;
        private DirectoryInfo searchDirectory = Runtime.Instance.AssetDir.Parent.CreateSubdirectory("Scenes");

        private bool showImGuiMetrics = false;
        private bool showImGuiStyle = false;

        public override void Tick()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New Scene"))
                    {
                        isNewSceneDialogOpen = true;
                    }
                    if (ImGui.MenuItem("Save Scene", "CTRL+S"))
                    {
                        if (Runtime.Instance.Context.GetSubSystem<WorldSystem>().CurrentSceneFile != null)
                        {
                            Runtime.Instance.ExecuteCommand(new Runtime.SaveSceneCommand());
                        }
                    }
                    if (ImGui.MenuItem("Load Scene"))
                    {
                        isLoadSceneDialogOpen = true;
                    }
                    ImGui.Separator();

                    if (ImGui.MenuItem("Options"))
                    {
                        UIController.Instance.Register(new SettingView(Core.Runtime.Instance.Settings, "Engine"));
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Debug"))
                {
                    if (ImGui.MenuItem("ImGui Metrics", "", ref showImGuiMetrics)) { }
                    if (ImGui.MenuItem("ImGui Style", "", ref showImGuiStyle)) { }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Window"))
                {
                    if (ImGui.MenuItem("Logs")) UIController.Instance.Register(new LogView());
                    if (ImGui.MenuItem("Performance")) UIController.Instance.Register(new PerformanceView());
                    if (ImGui.MenuItem("Scene")) UIController.Instance.Register(new ScenePropertiesView());

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }

            ImGuiWindowFlags flags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;

            var viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.Begin("###Dockspace", flags);
            ImGui.PopStyleVar();

            var id = ImGui.GetID("MyDockSpace");
            ImGui.DockSpace(id, new Vector2(0));
            ImGui.End();

            string hasChanged = RuntimeHelper.Instance.HasSceneChanged ? "*" : "";
            string newWindowTitle = $"Futura - { Runtime.Instance.Context.GetSubSystem<WorldSystem>().CurrentSceneFile?.Name }{hasChanged}";

            if(newWindowTitle != windowTitle)
            {
                Window.Instance.Title = newWindowTitle;
                windowTitle = newWindowTitle;
            }



            if (isLoadSceneDialogOpen)
            {
                if(FileDialog.Open("Open Scene", "*.scene", ref selectedFile, ref searchDirectory))
                {
                    if(selectedFile != null)
                    {
                        Runtime.Instance.ExecuteCommand(new Runtime.LoadSceneCommand(selectedFile));
                    }
                    isLoadSceneDialogOpen = false;
                }
            }

            if (isNewSceneDialogOpen)
            {
                if (FileDialog.Save("New Scene", "*.scene", ref selectedFile, ref searchDirectory))
                {
                    if (selectedFile != null)
                    {
                        Runtime.Instance.ExecuteCommand(new Runtime.LoadSceneCommand(selectedFile));
                    }
                    isNewSceneDialogOpen = false;
                }
            }

            if(showImGuiMetrics)
                ImGui.ShowMetricsWindow();
            if(showImGuiStyle)
                ImGui.ShowStyleEditor();
        }

    }
}
