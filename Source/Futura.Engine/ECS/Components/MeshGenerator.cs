using Futura.Engine.UserInterface.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.ECS.Components
{
    class MeshGenerator : IComponent, ICustomUserInterface
    {
        public enum MeshType { None, Quad, Cube, Sphere, Cylinder, Cone }


        public float X = 0.5f;
        public float Y = 0.5f;
        public float Z = 0.5f;

        public float Radius = 1.0f;
        public int Slices = 20;
        public int Stacks = 20;

        public float RadiusTop = 1.0f;
        public float RadiusBottom = 1.0f;
        public float Height = 1.0f;

        public MeshType GeometryType = MeshType.None;

        [JsonIgnore]
        [Futura.Ignore]
        public bool IsDirty { get; set; } = true;

        public bool Display()
        {
            bool hasChanged = false;

            if (PropertySerializerHelper.GetSerializer(typeof(MeshType))?.Serialize(this, nameof(GeometryType)) == true) hasChanged = true;

            switch (GeometryType)
            {
                case MeshType.None:
                    break;
                case MeshType.Quad:
                    if (PropertySerializerHelper.GetSerializer(typeof(float))?.Serialize(this, nameof(X)) == true) hasChanged = true;
                    if (PropertySerializerHelper.GetSerializer(typeof(float))?.Serialize(this, nameof(Y)) == true) hasChanged = true;
                    break;
                case MeshType.Cube:
                    if (PropertySerializerHelper.GetSerializer(typeof(float))?.Serialize(this, nameof(X)) == true) hasChanged = true;
                    if (PropertySerializerHelper.GetSerializer(typeof(float))?.Serialize(this, nameof(Y)) == true) hasChanged = true;
                    if (PropertySerializerHelper.GetSerializer(typeof(float))?.Serialize(this, nameof(Z)) == true) hasChanged = true;
                    break;
                case MeshType.Sphere:
                    if (PropertySerializerHelper.GetSerializer(typeof(float))?.Serialize(this, nameof(Radius)) == true) hasChanged = true;
                    if (PropertySerializerHelper.GetSerializer(typeof(int))?.Serialize(this, nameof(Slices)) == true) hasChanged = true;
                    if (PropertySerializerHelper.GetSerializer(typeof(int))?.Serialize(this, nameof(Stacks)) == true) hasChanged = true;
                    break;
                case MeshType.Cylinder:
                    if (PropertySerializerHelper.GetSerializer(typeof(float))?.Serialize(this, nameof(RadiusTop)) == true) hasChanged = true;
                    if (PropertySerializerHelper.GetSerializer(typeof(float))?.Serialize(this, nameof(RadiusBottom)) == true) hasChanged = true;
                    if (PropertySerializerHelper.GetSerializer(typeof(float))?.Serialize(this, nameof(Height)) == true) hasChanged = true;
                    if (PropertySerializerHelper.GetSerializer(typeof(int))?.Serialize(this, nameof(Slices)) == true) hasChanged = true;
                    if (PropertySerializerHelper.GetSerializer(typeof(int))?.Serialize(this, nameof(Stacks)) == true) hasChanged = true;
                    break;
                case MeshType.Cone:
                    if (PropertySerializerHelper.GetSerializer(typeof(float))?.Serialize(this, nameof(Radius)) == true) hasChanged = true;
                    if (PropertySerializerHelper.GetSerializer(typeof(float))?.Serialize(this, nameof(Height)) == true) hasChanged = true;
                    break;
                default:
                    break;
            }

            if (hasChanged) IsDirty = true;
            return hasChanged;
        }

    }
}
