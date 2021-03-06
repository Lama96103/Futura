using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Futura.Engine.UserInterface.Properties
{
    class StringSerializer : PropertySerializer
    {
        public override bool Serialize(object obj, FieldInfo field)
        {
            string value = (string)field.GetValue(obj);
            if(value == null) value = "";
            if (ImGui.InputText(GetName(field), ref value, 512))
            {
                field.SetValue(obj, value);
                return true;
            }
            return false;
        }
    }

    class FloatSerializer : PropertySerializer
    {
        public override bool Serialize(object obj, FieldInfo field)
        {
            RangeAttribute range = field.GetCustomAttribute<RangeAttribute>();
            float value = (float)field.GetValue(obj);
            
            if(range == null)
            {
                if (ImGui.DragFloat(GetName(field), ref value))
                {
                    field.SetValue(obj, value);
                    return true;
                }
                return false;
            }
            else
            {
                if (ImGui.SliderFloat(GetName(field), ref value, range.Min, range.Max))
                {
                    field.SetValue(obj, value);
                    return true;
                }
                return false;
            }
           
        }
    }

    class IntSerializer : PropertySerializer
    {
        public override bool Serialize(object obj, FieldInfo field)
        {
            int value = (int)field.GetValue(obj);
            if (ImGui.DragInt(GetName(field), ref value))
            {
                field.SetValue(obj, value);
                return true;
            }
            return false;
        }
    }


    class BoolSerializer : PropertySerializer
    {
        public override bool Serialize(object obj, FieldInfo field)
        {
            bool value = (bool)field.GetValue(obj);
            if (ImGui.Checkbox(GetName(field), ref value))
            {
                field.SetValue(obj, value);
                return true;
            }
            return false;
        }
    }

    class Vector2Serializer : PropertySerializer
    {
        public override bool Serialize(object obj, FieldInfo field)
        {
            Vector2 vector2 = (Vector2)field.GetValue(obj);
            if (ImGui.DragFloat2(GetName(field), ref vector2))
            {
                field.SetValue(obj, vector2);
                return true;
            }

            return false;
        }
    }

    class Vector3Serializer : PropertySerializer
    {
        public override bool Serialize(object obj, FieldInfo field)
        {
            Vector3 vector3 = (Vector3)field.GetValue(obj);
            if (ImGui.DragFloat3(GetName(field), ref vector3))
            {
                field.SetValue(obj, vector3);
                return true;
            }

            return false;
        }
    }

    class QuaternionSerializer : PropertySerializer
    {
        public override bool Serialize(object obj, FieldInfo field)
        {
            Quaternion q = (Quaternion)field.GetValue(obj);

            Vector3 rotation = q.ToEulerAngles();
            if (ImGui.DragFloat3(GetName(field), ref rotation))
            {
                q = rotation.FromEulerAngles();
                field.SetValue(obj, q);
                return true;
            }

            return false;
        }
    }

}
