using Futura.Engine.Core;
using Futura.Engine.Resources;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Futura.Engine.UserInterface
{
    class AssetView : View
    {
        public static Asset DragDropAsset = null;

        private ResourceManager manager = ResourceManager.Instance;


        public override void Init()
        { 

        }

        public override void Tick()
        {
            ImGui.Begin("Asset View##" + ID, ImGuiWindowFlags.MenuBar);
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.MenuItem("Material"))
                {
                    manager.CreateMaterial();
                }

                ImGui.EndMenuBar();
            }


            Asset[] userAssets = manager.LoadedAssets.ToArray();
            DirectoryInfo rootPath = manager.RootDirectory;
            DisplayDirectory(rootPath, userAssets);
                 
            ImGui.End();
        }

        private void DisplayDirectory(DirectoryInfo directory, Asset[] assets)
        {
            ImGuiTreeNodeFlags flags = 0;
            if (directory == manager.RootDirectory) flags = ImGuiTreeNodeFlags.DefaultOpen;
            if (ImGui.TreeNodeEx(directory.Name, flags))
            {
                foreach (DirectoryInfo subDir in directory.GetDirectories())
                    DisplayDirectory(subDir, assets);

                foreach(FileInfo fileInfo in directory.GetFiles())
                {
                    if (fileInfo.Extension == ResourceManager.MetaFileExtension) continue;
                    Asset asset = assets.Where(a => a.Path.FullName == fileInfo.FullName).FirstOrDefault();
                    if(asset == null)
                    {
                        Log.Warn(fileInfo.FullName + " asset is null");
                        continue;
                    }

                    if (ImGui.Selectable(asset.Path.Name))
                    {
                        RuntimeHelper.Instance.SelectedAsset = asset;
                    }

                    if (ImGui.BeginDragDropSource())
                    {
                        ImGui.TextDisabled(asset.Path.Name);
                        DragDropAsset = asset;
                        ImGui.SetDragDropPayload("PAYLOAD_ASSET_" + asset.AssetType.ToString(), IntPtr.Zero, 0);
                        ImGui.EndDragDropSource();
                    }
                }

                ImGui.TreePop();
            }
            


        }
    }
}
