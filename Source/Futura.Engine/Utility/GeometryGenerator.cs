using Futura.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Utility
{
    public static class GeometryGenerator
    {
        public static void GenerateCube(ref List<Vertex> vertices, ref List<uint> indices, float width = 1, float height = 1, float depth = 1)
        {
            uint index = (uint)vertices.Count;
        }

        public static void GenerateQuad(ref List<Vertex> vertices, ref List<uint> indices, float x = 1, float z = 1)
        {
            uint index = (uint)vertices.Count;
            Vector3 normal = Vector3.UnitY;

            vertices.Add(new Vertex(new Vector3(-x, 0, -z), normal));
            vertices.Add(new Vertex(new Vector3(-x, 0, z), normal));
            vertices.Add(new Vertex(new Vector3(x, 0, z), normal));
            vertices.Add(new Vertex(new Vector3(x, 0, -z), normal));

            indices.Add(index + 0);
            indices.Add(index + 1);
            indices.Add(index + 2);

            indices.Add(index + 0);
            indices.Add(index + 2);
            indices.Add(index + 3);
        }
        
    }
}
