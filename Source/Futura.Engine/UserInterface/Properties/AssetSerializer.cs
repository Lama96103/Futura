using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Futura.Engine.UserInterface.Properties
{
    //class GuidSerializer : PropertySerializer
    //{
    //    public override bool Serialize(object obj, FieldInfo field)
    //    {
    //        Guid value = (Guid)field.GetValue(obj);

    //        string name = value != null ? value.ToString() : "null";
    //        ImGui.InputText(GetName(field), ref name, 512, ImGuiInputTextFlags.ReadOnly);

    //        if (ImGui.BeginDragDropTarget())
    //        {
    //            var payload = ImGui.AcceptDragDropPayload("PAYLOAD_ASSET");
    //            unsafe
    //            {
    //                if (payload.NativePtr != null)
    //                {
    //                    FuturaAsset asset = AssetView.DragDropAsset;
    //                    if (asset.Type == AssetType.VoxelMesh)
    //                    {
    //                        field.SetValue(obj, asset.Guid);
    //                    }
    //                }
    //            }

    //            ImGui.EndDragDropTarget();
    //            return true;
    //        }
    //        return false;
    //    }
    //}
}
