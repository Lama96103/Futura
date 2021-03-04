using Futura.Engine.Resources;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Futura.Engine.UserInterface.Properties
{
    static class PropertySerializerHelper
    {
        private static Dictionary<Type, PropertySerializer> Serializer = new Dictionary<Type, PropertySerializer>()
        {
            { typeof(bool),  new BoolSerializer()},
            { typeof(float),  new FloatSerializer()},
            { typeof(int),  new IntSerializer()},
            { typeof(Vector2),  new Vector2Serializer()},
            { typeof(Vector3),  new Vector3Serializer()},
            { typeof(Quaternion),  new QuaternionSerializer()},
            { typeof(Mesh), new AssetSerializer<Mesh>() },
            { typeof(string), new StringSerializer() } //,
            // { typeof(Color), new ColorSerializer() }
        };

        public static PropertySerializer GetSerializer(Type type)
        {
            if(Serializer.TryGetValue(type, out PropertySerializer serializer))
            {
                return serializer;
            }
            return null;
        }
    }

    abstract class PropertySerializer 
    {
        public abstract bool Serialize(object obj, FieldInfo field);


        protected string GetName(FieldInfo field)
        {
            NameAttribute nameAttribute = field.GetCustomAttribute<NameAttribute>();
            if(nameAttribute != null)
            {
                return nameAttribute.Name;
            }
            return field.Name;
        }
    }

    

}
