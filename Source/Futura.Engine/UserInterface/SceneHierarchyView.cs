using Futura.Engine.Core;
using Futura.Engine.ECS;
using Futura.Engine.ECS.Components;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace Futura.Engine.UserInterface
{
    class SceneHierarchyView : View
    {
        string txt_WindowName;
        private Entity toDelete = null;

        private WorldSystem worldSystem;

        public override void Init()
        {
            worldSystem = Runtime.Instance.Context.GetSubSystem<WorldSystem>();
            txt_WindowName = $"Hierarchy##{ID}";
        }

        public override void Tick()
        {
            EcsWorld world = worldSystem.World;

            Begin(txt_WindowName, ImGuiWindowFlags.MenuBar);

            if (ImGui.BeginMenuBar())
            {
                if (ImGui.MenuItem("Create Entity"))
                {
                    var entity = world.CreateEntity();
                    entity.Get<Transform>();
                    RuntimeHelper.Instance.HasSceneChanged = true;
                }
                ImGui.EndMenuBar();
            }

            foreach (Entity entity in world.Entities)
            {
                RuntimeComponent comp = entity.Get<RuntimeComponent>();

                if (ImGui.Selectable(comp.Name + "##" + entity.ID.ToString(), entity == RuntimeHelper.Instance.SelectedEntity))
                {
                    RuntimeHelper.Instance.SelectedEntity = entity;
                }

                if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    toDelete = entity;
                    ImGui.OpenPopup("RightClickSceneHierarchy");
                }

            }

            if (ImGui.BeginPopup("RightClickSceneHierarchy"))
            {
                if (ImGui.MenuItem("Delete"))
                {
                    RuntimeHelper.Instance.SelectedEntity = null;
                    worldSystem.World.DestroyEntity(toDelete);
                    toDelete = null;
                    RuntimeHelper.Instance.HasSceneChanged = true;
                }
                ImGui.EndPopup();
            }

            End();




        }
    }
}
