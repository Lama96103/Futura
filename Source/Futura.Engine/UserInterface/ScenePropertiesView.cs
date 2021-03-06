﻿using Futura.Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.UserInterface
{
    class ScenePropertiesView : View
    {
        private WorldSystem worldSystem;

        private Type selectedSystemType = null;

        public override void Init()
        {
            worldSystem = Runtime.Instance.Context.GetSubSystem<WorldSystem>();
        }


        public override void Tick()
        {
            Begin($"Scene##{ID}", ref isOpen);

            Vector2 windowSize = ImGui.GetWindowSize();

            ImGui.Columns(2);

            ImGui.SetColumnWidth(0, windowSize.X - 125);
            ImGui.SetColumnWidth(1, 125);

            ImGui.TextDisabled("System");
            ImGui.NextColumn();
            ImGui.TextDisabled("Execution Order");
            ImGui.NextColumn();

            foreach (var s in worldSystem.WorldSystems.OrderBy(s => s.Value))
            {
                ImGui.Text(s.Key.Name);
                ImGui.NextColumn();

                int order = s.Value;
                if(ImGui.InputInt($"##{s.Key.FullName}", ref order))
                {
                    worldSystem.WorldSystems[s.Key] = order;
                    RuntimeHelper.Instance.HasSceneChanged = true;
                }
                ImGui.NextColumn();
            }

            if (ImGui.Button("Add System"))
            {
                ImGui.OpenPopup("SystemSelector");
            }

            if (ImGui.BeginPopup("SystemSelector"))
            {
                var components = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsClass && t.BaseType == typeof(Futura.Engine.ECS.EcsSystem) && !worldSystem.WorldSystems.ContainsKey(t));

                int id = 0;
                foreach (var comp in components)
                {
                    if (comp.GetCustomAttribute<IgnoreAttribute>() != null) continue;
                    if (ImGui.Selectable(comp.Name + "##" + id, comp == selectedSystemType, ImGuiSelectableFlags.DontClosePopups))
                    {
                        selectedSystemType = comp;
                    }
                    id++;
                }

                if (selectedSystemType != null && ImGui.Button("Add"))
                {
                    worldSystem.WorldSystems.Add(selectedSystemType, 100);
                    RuntimeHelper.Instance.HasSceneChanged = true;
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            End();
        }
    }
}
