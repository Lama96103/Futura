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


            var allAssets = manager.LoadedAssets;

            var userAssets = allAssets;
            // var editorAssets = allAssets.Where(s => s.EditorPath == "Built-In");

            if (ImGui.TreeNodeEx("Assets", ImGuiTreeNodeFlags.DefaultOpen))
            {
                foreach (var asset in userAssets.ToArray())
                {
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

            ImGui.End();
        }
    }
}
