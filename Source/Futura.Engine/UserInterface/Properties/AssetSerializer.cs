using Futura.Engine.Rendering;
using Futura.Engine.Resources;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Futura.Engine.UserInterface.Properties
{
    class AssetSerializer<T> : PropertySerializer where T : Asset
    {
        public override bool Serialize(object obj, FieldInfo field)
        {
            Asset value = (Asset)field.GetValue(obj);

            string name = value != null ? value.Path.Name : "null";
            ImGui.InputText(GetName(field), ref name, 512, ImGuiInputTextFlags.ReadOnly);

            if (ImGui.BeginDragDropTarget())
            {
                Asset asset = AssetView.DragDropAsset;
                if(asset != null)
                {
                    var payload = ImGui.AcceptDragDropPayload("PAYLOAD_ASSET_" + asset.AssetType.ToString());
                    unsafe
                    {
                        if (payload.NativePtr != null && asset.GetType() == typeof(T))
                        {
                            field.SetValue(obj, (T)asset);
                            AssetView.DragDropAsset = null;
                        }
                    }
                }

                ImGui.EndDragDropTarget();
                return true;
            }
            return false;
        }
    }
}
