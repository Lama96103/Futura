using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Rendering.Gizmo
{
    class TransformPositionHandle : TransformHandle
    {
        public override void Init(ResourceFactory factory)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            Utility.GeometryGenerator.GenerateCylinder(ref vertices, ref indices, 0.2f, 0.2f, 5f, 5, 5);

            for (int i = 0; i < vertices.Count; i++)
            {
                Vertex v = vertices[i];
                v.Position += new System.Numerics.Vector3(0, 3.5f, 0);

                vertices[i] = v;
            }

            List<Vertex> coneVertices = new List<Vertex>();
            List<uint> coneIndices = new List<uint>();
            Utility.GeometryGenerator.GenerateCone(ref coneVertices, ref coneIndices, 0.4f, 1, 5, 5);

            for (int i = 0; i < coneVertices.Count; i++)
            {
                Vertex v = coneVertices[i];
                v.Position += new System.Numerics.Vector3(0, 6.0f, 0);

                coneVertices[i] = v;
            }

            uint baseIndex = (uint)vertices.Count;
            for (int i = 0; i < coneIndices.Count; i++)
            {
                coneIndices[i] += baseIndex;
            }

            vertices.AddRange(coneVertices);
            indices.AddRange(coneIndices);


            renderable = new Renderable();
            renderable.Load(RenderAPI.Instance, vertices.ToArray(), indices.ToArray());
        }
    }
}
