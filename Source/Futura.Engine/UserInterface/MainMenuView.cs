﻿using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.UserInterface
{
    public class MainMenuView : View
    {
        public override void Tick()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New Project")) { }
                    if (ImGui.MenuItem("Open Project")) {  }
                    ImGui.Separator();
                  
                    
                    if (ImGui.MenuItem("Editor Settings"))
                    {
                        // UIController.Instance.Register(new SettingView(EditorApp.Instance.SettingsController, "Editor"));
                    }
                    ImGui.EndMenu();
                }

            

              

                if (ImGui.BeginMenu("Debug"))
                {
                    if (ImGui.MenuItem("Log Assemblies"))
                    {
                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            Console.WriteLine(assembly.FullName);
                        }
                    }
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

        }
    }
}
