using Futura.Engine.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Rendering
{
    [Serializable]
    public struct Vertex : ISerialize
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;

        public const uint Size = 32;


        public Vertex(Vector3 position, Vector3 normal, Vector2 uv)
        {
            this.Position = position;
            this.Normal = normal;
            this.UV = uv;
        }

        public Vertex(BinaryReader reader)
        {
            Position = VectorExtension.ReadVector3(reader);
            Normal = VectorExtension.ReadVector3(reader);
            UV = VectorExtension.ReadVector2(reader);
        }

        public static VertexLayoutDescription GetLayoutDescription()
        {
            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("vertexPosition", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                   new VertexElementDescription("vertexNormal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                new VertexElementDescription("vertexUV", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));
            return vertexLayout;
        }

        public void Write(BinaryWriter writer)
        {
            Position.Write(writer);
            Normal.Write(writer);
            UV.Write(writer);
        }

        public void Read(BinaryReader reader)
        {
            Position = VectorExtension.ReadVector3(reader);
            Normal = VectorExtension.ReadVector3(reader);
            UV = VectorExtension.ReadVector2(reader);
        }
    }
}
