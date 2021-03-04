﻿using Futura.Engine.Core;
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
                        ImGui.SetDragDropPayload("PAYLOAD_ASSET", IntPtr.Zero, 0);
                        DragDropAsset = asset;
                        ImGui.EndDragDropSource();
                    }
                }
                ImGui.TreePop();
            }

            //if (ImGui.TreeNode("Built-In"))
            //{
            //    foreach (var asset in editorAssets)
            //    {
            //        if (ImGui.Selectable(asset.Name))
            //        {
            //            EditorApp.Instance.SelectedAsset = asset;
            //        }

            //        if (ImGui.BeginDragDropSource())
            //        {
            //            ImGui.TextDisabled(asset.Name);
            //            ImGui.SetDragDropPayload("PAYLOAD_ASSET", IntPtr.Zero, 0);
            //            DragDropAsset = asset;
            //            ImGui.EndDragDropSource();
            //        }
            //    }
            //    ImGui.TreePop();
            //}


            ImGui.End();
        }
    }
}