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
        public float index;
        public float ambientOcculssion;

        public const uint Size = 32;


        public Vertex(float x, float y, float z, float index, float ao, Vector3 normal)
        {
            Position = new Vector3(x, y, z);
            this.index = index;
            this.ambientOcculssion = ao;
            this.Normal = normal;
        }

        public Vertex(Vector3 position, Vector3 normal)
        {
            this.Position = position;
            this.Normal = normal;
            this.ambientOcculssion = 0;
            this.index = 0;
        }

        public Vertex(BinaryReader reader)
        {
            Position = VectorExtension.Read(reader);
            Normal = VectorExtension.Read(reader);
            index = reader.ReadSingle();
            ambientOcculssion = reader.ReadSingle();
        }

        public static VertexLayoutDescription GetLayoutDescription()
        {
            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("vertexPosition", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                   new VertexElementDescription("normalVector", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                new VertexElementDescription("vertexIndex", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float1),
                new VertexElementDescription("vertexAmbientOcclusion", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float1));
            return vertexLayout;
        }

        public void Write(BinaryWriter writer)
        {
            Position.Write(writer);
            Normal.Write(writer);

            writer.Write(index);
            writer.Write(ambientOcculssion);
        }

        public void Read(BinaryReader reader)
        {
            Position = VectorExtension.Read(reader);
            Normal = VectorExtension.Read(reader);
            index = reader.ReadSingle();
            ambientOcculssion = reader.ReadSingle();
        }
    }
}
