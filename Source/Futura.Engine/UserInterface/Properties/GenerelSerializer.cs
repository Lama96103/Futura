using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Futura.Engine.UserInterface.Properties
{
    class ColorSerializer : PropertySerializer
    {
        public override bool Serialize(object obj, FieldInfo field)
        {
            Color color = (Color)field.GetValue(obj);
            Vector4 data = color.RawData;
            if (ImGui.ColorEdit4(GetName(field), ref data))
            {
                color.RawData = data;
                field.SetValue(obj, color);
                return true;
            }

            return false;
        }
    }

    class EnumSerializer : PropertySerializer
    {
        public override bool Serialize(object obj, FieldInfo field)
        {
            Enum value = field.GetValue(obj) as Enum;
            Array possibleValues = Enum.GetValues(field.FieldType);

            int currentValue = 0;
            string[] possibleValuesString = new string[possibleValues.Length];

            for (int i = 0; i < possibleValues.Length; i++)
            {
                object v = possibleValues.GetValue(i);
                possibleValuesString[i] = v.ToString();
                if (value.ToString() == v.ToString()) currentValue = i;

            }

            if(ImGui.Combo(GetName(field), ref currentValue, possibleValuesString, possibleValuesString.Length))
            {
                field.SetValue(obj, currentValue);
            }

            return false;
        }
    }
}
