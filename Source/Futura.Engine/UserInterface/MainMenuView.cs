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

        public override void Tick()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Save")) 
                    {
                        Runtime.Instance.Context.GetSubSystem<WorldSystem>().Save(Runtime.Instance.CurrentScene);
                    }
                    if (ImGui.MenuItem("Load")) 
                    {
                    }
                    ImGui.Separator();
                  
                    
                    
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Options"))
                {
                    if (ImGui.MenuItem("Engine Settings"))
                    {
                        UIController.Instance.Register(new SettingView(Core.Runtime.Instance.Settings, "Engine"));
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Window"))
                {
                    if (ImGui.MenuItem("Logs")) UIController.Instance.Register(new LogView());

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
            string newWindowTitle = $"Futura - {Runtime.Instance.CurrentScene.Name}{hasChanged}";

            if(newWindowTitle != windowTitle)
            {
                Window.Instance.Title = newWindowTitle;
                windowTitle = newWindowTitle;
            }
        }
    }
}
