using Futura.ECS;
using Futura.Engine.Components;
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

        private bool didChange = false;

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
            didChange = false;
        }

        private void EditorApp_EntitySelectionChanged(object sender, EntitySelectionChangedEventArgs e)
        {
            entity = e.Entity;
            asset = null;
            imageSource = IntPtr.Zero;
            didChange = false;
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
                didChange = true;
            }

            ImGui.Separator();


            // Content
            foreach (IComponent component in entity.GetAllComponents())
            {
                if (component.GetType().GetCustomAttribute<IgnoreAttribute>() != null) continue;

                ImGui.Text(component.GetType().Name);

                var fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
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
                            didChange = true;
                            if(component.GetType() == typeof(Transform)) ((Transform)component).UpdateTransform();
                            if (component.GetType() == typeof(MeshGenerator)) ((MeshGenerator)component).IsDirty = true;

                        }
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
                didChange = true;
            }

            bool mipmap = texture.UseMipMap;
            if (ImGui.Checkbox("MipMap", ref mipmap))
            {
                texture.UseMipMap = mipmap;
                didChange = true;
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
                    if (serializer.Serialize(material, f)) didChange = true;
                }
            }
        }

        private void SaveChanges()
        {
            if(entity != null)
            {
                Runtime.Instance.Context.GetSubSystem<WorldSystem>().Save(Runtime.Instance.CurrentScene);
                didChange = false;
            }
            else if(asset != null)
            {
                Resources.ResourceManager.Instance.Save(asset);

                asset.Unload();
                asset.Load();
                didChange = false;
            }
        }

        public override void Tick()
        {
            ImGuiWindowFlags flags = ImGuiWindowFlags.MenuBar;
            if (didChange) flags |= ImGuiWindowFlags.UnsavedDocument;

            ImGui.Begin(txt_WindowName, ref isOpen, flags);

            if (didChange)
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.MenuItem("Save"))
                    {
                        SaveChanges();
                    }
                    ImGui.EndMenuBar();
                }
            }

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
