using Futura.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Resources
{
    public class Mesh : Asset
    {

        internal Mesh(Guid identifier, FileInfo path) : base(identifier, AssetType.Mesh, path)
        {
        }

        private Vertex[] vertices;
        private uint[] indices;



        

        public override void Write(BinaryWriter writer)
        {
            writer.Write(vertices.Length);
            foreach (Vertex v in vertices) v.Write(writer);
            writer.Write(indices.Length);
            foreach (uint i in indices) writer.Write(i);
        }

        public override void Read(BinaryReader reader)
        {
            int vertexLength = reader.ReadInt32();
            vertices = new Vertex[vertexLength];
            for (int i = 0; i < vertexLength; i++) vertices[i] = new Vertex(reader);
            
            int indexLength = reader.ReadInt32();
            indices = new uint[indexLength];
            for (int i = 0; i < indexLength; i++) indices[i] = reader.ReadUInt32();
        }
    }
}
