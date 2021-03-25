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
            Begin("Asset View##" + ID, ImGuiWindowFlags.MenuBar);
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.MenuItem("Material"))
                {
                    manager.CreateMaterial();
                }

                ImGui.EndMenuBar();
            }


            Asset[] userAssets = manager.LoadedAssets.ToArray();

            Profiler.StartTimeMeasure(typeof(AssetView).FullName + ".CreateFolderStructure()");
            Folder rootFolder = new Folder(manager.RootDirectory.Name);
            foreach (Asset a in userAssets)
            {
                string[] folders = a.Path.Directory.FullName.Split('\\');
                int rootIndex = 0;
                for (int i = 0; i < folders.Length; i++)
                {
                    if (folders[i] == manager.RootDirectory.Name)
                    {
                        rootIndex = i;
                        break;
                    }
                }

                rootFolder.Insert(a, folders.Skip(rootIndex));
            }
            Profiler.StopTimeMeasure(typeof(AssetView).FullName + ".CreateFolderStructure()");

            Profiler.StartTimeMeasure(typeof(AssetView).FullName + ".DisplayDirectory()");
            DisplayDirectory(rootFolder);
            Profiler.StopTimeMeasure(typeof(AssetView).FullName + ".DisplayDirectory()");
                 
            End();
        }

        private void DisplayDirectory(Folder folder)
        {
            if (ImGui.TreeNodeEx(folder.Name))
            {
                foreach (Folder subDir in folder.SubFolders)
                    DisplayDirectory(subDir);

                foreach (Asset asset in folder.Assets)
                {
                    if (ImGui.Selectable(asset.Path.Name, RuntimeHelper.Instance.SelectedAsset == asset, asset.IsDeleted ? ImGuiSelectableFlags.Disabled : ImGuiSelectableFlags.None))
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

        class Folder
        {
            public string Name = "";
            public List<Folder> SubFolders = new List<Folder>();
            public List<Asset> Assets = new List<Asset>();

            public Folder(string name)
            {
                Name = name;
            }

            public void Insert(Asset asset, IEnumerable<string> folders)
            {
                if(folders.Count() == 1)
                {
                    Assets.Add(asset);
                }
                else
                {
                    string subFolderName = folders.ElementAt(1);
                    var subFoldersArray = folders.Skip(1);

                    Folder subFolder = null;
                    foreach(Folder s in SubFolders)
                    {
                        if(s.Name == subFolderName)
                        {
                            subFolder = s;
                        }
                    }
                    if(subFolder == null)
                    {
                        subFolder = new Folder(subFolderName);
                        SubFolders.Add(subFolder);
                    }

                    subFolder.Insert(asset, subFoldersArray);
                }
            }


        }
    }
}
