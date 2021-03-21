using Futura.Engine.Core;
using Futura.Engine.ECS;
using Futura.Engine.ECS.Components;
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

            if (ImGui.Checkbox("##EnableAsset", ref baseComponent.IsEnabled)) RuntimeHelper.Instance.HasSceneChanged = true;
            ImGui.SameLine();
            if (ImGui.InputText("Name##Header", ref name, 100))
            {
                baseComponent.Name = name;
                RuntimeHelper.Instance.HasSceneChanged = true;
            }

            PropertySerializerHelper.GetSerializer(typeof(EntityTags)).Serialize(baseComponent, nameof(baseComponent.EntityTags));


            ImGui.Separator();

            // Content
            foreach (IComponent component in entity.GetAllComponents())
            {
                Type componentType = component.GetType();
                if (componentType.GetCustomAttribute<IgnoreAttribute>() != null) continue;


                bool isOpen = true;
                if (ImGui.CollapsingHeader(componentType.Name, ref isOpen, ImGuiTreeNodeFlags.DefaultOpen))
                {
                    if (componentType.GetInterface("ICustomUserInterface") != null)
                    {
                        ICustomUserInterface ui = (ICustomUserInterface)component;
                        ui.Display();
                    }
                    else
                    {
                        var fields = componentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var f in fields)
                        {
                            if (!f.IsPublic && f.GetCustomAttribute<SerializeField>() == null) continue;
                            if (f.GetCustomAttribute<IgnoreAttribute>() != null) continue;
                            var serializer = PropertySerializerHelper.GetSerializer(f.FieldType);
                            if (serializer == null)
                            {
                                ImGui.LabelText(f.Name, "TODO - " + f.FieldType.Name);
                            }
                            else
                            {
                                bool change = serializer.Serialize(component, f);
                                if (change)
                                {
                                    RuntimeHelper.Instance.HasSceneChanged = true;
                                    if (component.GetType() == typeof(Transform)) ((Transform)component).UpdateTransform();
                                }
                            }
                        }
                    }
                }

                if (isOpen == false)
                {
                    entity.RemoveComponent(componentType);
                }
            }

            ImGui.Separator();

            float i = ImGui.GetWindowSize().X / 2;
            ImGui.Indent(i- 50);
            if (ImGui.Button("Add Component"))
            {
                ImGui.OpenPopup("ComponentSelector");
            }

            if (ImGui.BeginPopup("ComponentSelector"))
            {
                var components = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsClass && t.GetInterface(typeof(IComponent).FullName) != null);


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
                    RuntimeHelper.Instance.HasSceneChanged = true;
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
            if (ImGui.InputText("Name##Header", ref name, 100)) {}

            if (asset.HasAssetChanged)
            {
                ImGui.SameLine();
                if (ImGui.Button("Save"))
                {
                    ResourceManager.Instance.Save(asset);
                    asset.HasAssetChanged = false;
                }
            }
            
            
            ImGui.Separator();

            // Content
            switch (asset.AssetType)
            {
                case AssetType.Material:
                    DisplayMaterial((Material)asset);
                    break;
                case AssetType.Mesh:
                    DisplayMeshAsset((Mesh)asset);
                    break;
                case AssetType.Texture2d:
                    DisplayTexture((Texture2D)asset);
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
            bool srgb = texture.IsSRGB;
            if(ImGui.Checkbox("SRGB", ref srgb))
            {
                texture.IsSRGB = srgb;
                texture.HasAssetChanged = true;
            }

            bool mipmap = texture.UseMipMap;
            if (ImGui.Checkbox("MipMap", ref mipmap))
            {
                texture.UseMipMap = mipmap;
                texture.HasAssetChanged = true;
            }

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

        private void DisplayMaterial(Material material)
        {
            var fields = material.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var f in fields)
            {
                if (!f.IsPublic && f.GetCustomAttribute<SerializeField>() == null) continue;
                if (f.GetCustomAttribute<IgnoreAttribute>() != null) continue;
                var serializer = PropertySerializerHelper.GetSerializer(f.FieldType);
                if (serializer == null)
                {
                    ImGui.LabelText(f.Name, "TODO - " + f.FieldType.Name);
                }
                else
                {
                    if (serializer.Serialize(material, f)) material.HasAssetChanged = true;
                }
            }
        }



        public override void Tick()
        {
            ImGuiWindowFlags flags = 0;
            ImGui.Begin(txt_WindowName, ref isOpen, flags);

        
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
