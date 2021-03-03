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
                if (ImGui.InputFloat(GetName(field), ref value))
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
            if (ImGui.InputInt(GetName(field), ref value))
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
            if (ImGui.InputFloat2(GetName(field), ref vector2))
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
            if (ImGui.InputFloat3(GetName(field), ref vector3))
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

            Vector3 rotation = new Vector3();

            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            rotation.X = (float)Math.Atan2(sinr_cosp, cosr_cosp).ToDegree();

            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
                rotation.Y = (float)Math.CopySign(Math.PI / 2, sinp).ToDegree(); // use 90 degrees if out of range
            else
                rotation.Y = (float)Math.Asin(sinp).ToDegree();

            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            rotation.Z = (float)Math.Atan2(siny_cosp, cosy_cosp).ToDegree();

            if (ImGui.InputFloat3(GetName(field), ref rotation))
            {
                q = Quaternion.CreateFromYawPitchRoll(rotation.X.ToRadians(), rotation.Y.ToRadians(), rotation.Z.ToRadians());
                field.SetValue(obj, q);
                return true;
            }

            return false;
        }
    }

}
