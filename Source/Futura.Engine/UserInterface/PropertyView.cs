using Futura.ECS;
using Futura.Engine.Core;
using Futura.Engine.ECS;
using Futura.Engine.Rendering;
using Futura.Engine.Resources;
using Futura.Engine.UserInterface.Properties;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Futura.Engine.UserInterface
{
    class PropertyView : View
    {
        private string txt_WindowName;

        private Entity entity = null;
        private Asset asset = null;

        IntPtr imageSource = IntPtr.Zero;

        private Type selectedComponentType = null;

        private WorldSystem worldSystem;

        public override void Init()
        {
            worldSystem = Runtime.Instance.Context.GetSubSystem<WorldSystem>();
            RuntimeHelper.Instance.EntitySelectionChanged += EditorApp_EntitySelectionChanged;
            RuntimeHelper.Instance.AssetSelectionChanged += EditorApp_AssetSelectionChanged;

            txt_WindowName = $"Property##{ID}";
        }

        private void EditorApp_AssetSelectionChanged(object sender, AssetSelectionChangedEventArgs e)
        {
            entity = null;
            asset = e.Asset;
            imageSource = IntPtr.Zero;
        }

        private void EditorApp_EntitySelectionChanged(object sender, EntitySelectionChangedEventArgs e)
        {
            entity = e.Entity;
            asset = null;
            imageSource = IntPtr.Zero;
        }

        private void DisplayEntity()
        {
            // Header of the panel
            RuntimeComponent baseComponent = entity.GetComponent<RuntimeComponent>();

            string name = baseComponent.Name;
            bool enabled = baseComponent.IsEnabled;

            if (ImGui.Checkbox("##EnableAsset", ref baseComponent.IsEnabled)) ;
            ImGui.SameLine();
            if (ImGui.InputText("Name##Header", ref name, 100))
            {
                baseComponent.Name = name;
            }

            //if (ImGui.Checkbox("Selectable##SelectableAsset", ref baseComponent.IsSelectable)) EditorApp.Instance.SceneHasChanged = true;
            ImGui.Separator();


            // Content
            foreach (IComponent component in entity.GetAllComponents())
            {
                if (component.GetType().GetCustomAttribute<IgnoreAttribute>() != null) continue;

                ImGui.Text(component.GetType().Name);

                var fields = component.GetType().GetFields();
                foreach (var f in fields)
                {
                    if (f.GetCustomAttribute<IgnoreAttribute>() != null) continue;
                    var serializer = PropertySerializerHelper.GetSerializer(f.FieldType);
                    if (serializer == null)
                    {
                        ImGui.LabelText(f.Name, "TODO - " + f.FieldType.Name);
                    }
                    else
                    {
                        bool didChange = serializer.Serialize(component, f);
                        //if (didChange)
                            //EditorApp.Instance.SceneHasChanged = true;
                    }
                }

                ImGui.Separator();
            }

            if (ImGui.Button("Add Component"))
            {
                ImGui.OpenPopup("ComponentSelector");
            }

#if DEBUG
            if (ImGui.Button("Destroy"))
            {
                worldSystem.World.DestroyEntity(entity);
                //EditorApp.Instance.SelectedEntity = null;
            }
#endif

            if (ImGui.BeginPopup("ComponentSelector"))
            {
                var components = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsClass && t.GetInterface("Futura.ECS.IComponent") != null);



                int id = 0;
                foreach (var comp in components)
                {
                    if (comp.GetCustomAttribute<IgnoreAttribute>() != null) continue;
                    if (ImGui.Selectable(comp.Name + "##" + id, comp == selectedComponentType, ImGuiSelectableFlags.DontClosePopups))
                    {
                        selectedComponentType = comp;
                    }
                    id++;
                }

                if (selectedComponentType != null && ImGui.Button("Add"))
                {
                    entity.AddComponent(selectedComponentType);
                    Log.Debug("Add component " + selectedComponentType.Name);
                    //EditorApp.Instance.SceneHasChanged = true;
                    selectedComponentType = null;
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }


        private void DisplayAsset()
        {
            // Header
            string name = asset.Path.Name;
            if (ImGui.InputText("Name##Header", ref name, 100))
            {
            }
            ImGui.Separator();

            // Content
            switch (asset.AssetType)
            {
                case AssetType.Material:
                    break;
                case AssetType.Mesh:
                    DisplayMeshAsset((Mesh)asset);
                    break;
                case AssetType.Texture2d:
                    //DisplayTexture((Texture2D)asset);
                    break;
                default:
                    break;
            }

#if DEBUG
            ImGui.Separator();

            ImGui.TextDisabled("Guid: ");
            ImGui.SameLine();
            ImGui.TextDisabled(asset.Identifier.ToString());
            ImGui.TextDisabled("IsLoaded: ");
            ImGui.SameLine();
            ImGui.TextDisabled(asset.IsLoaded.ToString());

            if (asset.IsLoaded)
            {
                if (ImGui.Button("Unload")) asset.Unload();
            }
            else
            {
                if (ImGui.Button("Load")) asset.Load();
            }
#endif
        }

        private void DisplayMeshAsset(Mesh mesh)
        {
            ImGui.TextDisabled("Vertices: ");
            ImGui.SameLine();
            ImGui.TextDisabled(mesh.VertexCount.ToString());

            ImGui.TextDisabled("Indices: ");
            ImGui.SameLine();
            ImGui.TextDisabled(mesh.IndexCount.ToString());
        }

        private void DisplayTexture(Texture2D texture)
        {
            ImGui.TextDisabled("Width: ");
            ImGui.SameLine();
            ImGui.TextDisabled(texture.Width.ToString());

            ImGui.TextDisabled("Height: ");
            ImGui.SameLine();
            ImGui.TextDisabled(texture.Height.ToString());

            ImGui.TextDisabled("Format: ");
            ImGui.SameLine();
            ImGui.TextDisabled(texture.Format.ToString());

            imageSource = ImGuiController.Instance.GetOrCreateImGuiBinding(texture.Handle);

            Vector2 size = ImGui.GetWindowSize();


            float sizeX = (float)texture.Width / (size.X - 10);
            float sizeY = (float)texture.Height / sizeX;

            ImGui.Image(imageSource, new Vector2((size.X - 10), sizeY));
        }

        public override void Tick()
        {
            ImGui.Begin(txt_WindowName);

            if (entity == null && asset == null)
            {
                selectedComponentType = null;
                ImGui.TextDisabled("Select Entity/Asset to view content");
            }
            else
            {
                if (entity != null)
                    DisplayEntity();
                else
                    DisplayAsset();
            }

            ImGui.End();
        }

  
    }
}
