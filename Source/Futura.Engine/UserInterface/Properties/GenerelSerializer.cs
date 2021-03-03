using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Futura.Engine.UserInterface.Properties
{
    //class ColorSerializer : PropertySerializer
    //{
    //    public override bool Serialize(object obj, FieldInfo field)
    //    {
    //        Color color = (Color)field.GetValue(obj);
    //        Vector4 colorVector = color.ToVector4();
    //        if (ImGui.ColorEdit4(field.Name, ref colorVector))
    //        {
    //            field.SetValue(obj, new Color(colorVector));
    //            return true;
    //        }

    //        return false;
    //    }
    //}
}
