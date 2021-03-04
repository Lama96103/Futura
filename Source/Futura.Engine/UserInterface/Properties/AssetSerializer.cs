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
                var payload = ImGui.AcceptDragDropPayload("PAYLOAD_ASSET");
                unsafe
                {
                    if (payload.NativePtr != null)
                    {
                        Asset asset = AssetView.DragDropAsset;
                        AssetType type = AssetType.Unkown;
                        if (typeof(T) == typeof(Mesh)) type = AssetType.Mesh;
                        if (typeof(T) == typeof(Material)) type = AssetType.Material;
                        if (typeof(T) == typeof(Mesh)) type = AssetType.Mesh;
                        if (asset.AssetType == type)
                        {
                            field.SetValue(obj, (T)asset);
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
