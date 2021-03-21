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
        public static void GenerateCube(ref List<Vertex> vertices, ref List<uint> indices, float x = 1, float y = 1, float z = 1)
        {
            uint index = (uint)vertices.Count;

            Vector2 uv = new Vector2();

            // Top
            Vector3 normal = Vector3.UnitY;
            vertices.Add(new Vertex(new Vector3(-x, y, -z), normal, uv));
            vertices.Add(new Vertex(new Vector3(-x, y, z), normal, uv));
            vertices.Add(new Vertex(new Vector3(x, y, z), normal, uv));
            vertices.Add(new Vertex(new Vector3(x, y, -z), normal, uv));

            indices.Add(index + 0);
            indices.Add(index + 1);
            indices.Add(index + 2);

            indices.Add(index + 0);
            indices.Add(index + 2);
            indices.Add(index + 3);
            index += 4;


            // Bottom
            normal = -Vector3.UnitY;
            vertices.Add(new Vertex(new Vector3(-x, -y, -z), normal, uv));
            vertices.Add(new Vertex(new Vector3(-x, -y, z), normal, uv));
            vertices.Add(new Vertex(new Vector3(x, -y, z), normal, uv));
            vertices.Add(new Vertex(new Vector3(x, -y, -z), normal, uv));

            indices.Add(index + 0);
            indices.Add(index + 2);
            indices.Add(index + 1);

            indices.Add(index + 0);
            indices.Add(index + 3);
            indices.Add(index + 2);
            index += 4;


            // Right
            normal = -Vector3.UnitX;
            vertices.Add(new Vertex(new Vector3(-x, -y, -z), normal, uv));
            vertices.Add(new Vertex(new Vector3(-x, -y, z), normal, uv));
            vertices.Add(new Vertex(new Vector3(-x, y, z), normal, uv));
            vertices.Add(new Vertex(new Vector3(-x, y, -z), normal, uv));

            indices.Add(index + 0);
            indices.Add(index + 1);
            indices.Add(index + 2);

            indices.Add(index + 0);
            indices.Add(index + 2);
            indices.Add(index + 3);
            index += 4;

            // Left
            normal = Vector3.UnitX;
            vertices.Add(new Vertex(new Vector3(x, -y, -z), normal, uv));
            vertices.Add(new Vertex(new Vector3(x, -y, z), normal, uv));
            vertices.Add(new Vertex(new Vector3(x, y, z), normal, uv));
            vertices.Add(new Vertex(new Vector3(x, y, -z), normal, uv));

            indices.Add(index + 0);
            indices.Add(index + 2);
            indices.Add(index + 1);

            indices.Add(index + 0);
            indices.Add(index + 3);
            indices.Add(index + 2);
            index += 4;

            // Back
            normal = -Vector3.UnitZ;
            vertices.Add(new Vertex(new Vector3(-x, -y, -z), normal, uv));
            vertices.Add(new Vertex(new Vector3(x, -y, -z), normal, uv));
            vertices.Add(new Vertex(new Vector3(x, y, -z), normal, uv));
            vertices.Add(new Vertex(new Vector3(-x, y, -z), normal, uv));

            indices.Add(index + 0);
            indices.Add(index + 2);
            indices.Add(index + 1);

            indices.Add(index + 0);
            indices.Add(index + 3);
            indices.Add(index + 2);
            index += 4;

            // Front
            normal = Vector3.UnitZ;
            vertices.Add(new Vertex(new Vector3(-x, -y, z), normal, uv));
            vertices.Add(new Vertex(new Vector3(x, -y, z), normal, uv));
            vertices.Add(new Vertex(new Vector3(x, y, z), normal, uv));
            vertices.Add(new Vertex(new Vector3(-x, y, z), normal, uv));

            indices.Add(index + 0);
            indices.Add(index + 1);
            indices.Add(index + 2);

            indices.Add(index + 0);
            indices.Add(index + 2);
            indices.Add(index + 3);
        }

        public static void GenerateQuad(ref List<Vertex> vertices, ref List<uint> indices, float x = 1, float z = 1)
        {
            uint index = (uint)vertices.Count;
            Vector3 normal = Vector3.UnitY;

            vertices.Add(new Vertex(new Vector3(-x, 0, -z), normal, new Vector2(0, 0)));
            vertices.Add(new Vertex(new Vector3(-x, 0, z), normal, new Vector2(0, 1)));
            vertices.Add(new Vertex(new Vector3(x, 0, z), normal, new Vector2(1, 1)));
            vertices.Add(new Vertex(new Vector3(x, 0, -z), normal, new Vector2(1, 0)));

            indices.Add(index + 0);
            indices.Add(index + 1);
            indices.Add(index + 2);

            indices.Add(index + 0);
            indices.Add(index + 2);
            indices.Add(index + 3);
        }
        
        /// <summary>
        /// Creates a sphere into the two list,
        /// not clear if it works if there are already some vertices inside not tested
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <param name="radius"></param>
        /// <param name="slices"></param>
        /// <param name="stacks"></param>
        public static void GenerateSphere(ref List<Vertex> vertices, ref List<uint> indices, float radius = 1.0f, int slices = 20, int stacks = 20)
        {
            uint index = (uint)vertices.Count;

            Vector3 normal = new Vector3(0, 1, 0);
            Vector3 tangent = new Vector3(1, 0, 0);

            vertices.Add(new Vertex(new Vector3(0, radius, 0), normal, new Vector2()));

            float phiStep = (float)(Math.PI / (double)stacks);
            float thetaStep = (float)(2.0 * Math.PI / (double)slices);

            for (int i = 1; i <= stacks; i++)
            {
                float phi = i * phiStep;

                for (int j = 0; j <= slices; j++)
                {
                    float theta = j * thetaStep;
                    Vector3 p = new Vector3(
                        (radius * (float)Math.Sin(phi) * (float)Math.Cos(theta)),
                        (radius * (float)Math.Cos(phi)),
                        (radius * (float)Math.Sin(phi) * (float)Math.Sin(theta)));

                    Vector3 t = new Vector3(-radius * (float)Math.Sin(phi) * (float)Math.Sin(theta), 0, radius * (float)Math.Sin(phi) * (float)Math.Cos(theta));
                    t = Vector3.Normalize(t);
                    Vector3 n = Vector3.Normalize(p);
                    Vector2 uv = new Vector2(theta / ((float)Math.PI * 2), phi / (float)Math.PI);
                    vertices.Add(new Vertex(p, n, uv));
                }
            }

            normal = new Vector3(0, -1, 0);
            tangent = new Vector3(1, 0, 0);
            vertices.Add(new Vertex(new Vector3(0, -radius, 0), normal, new Vector2(1)));

            for (uint i = 1; i <= slices; i++)
            {
                indices.Add(index + 0);
                indices.Add(index + i + 1);
                indices.Add(index + i);
            }

            uint baseIndex = index + 1;
            uint ringVertexCount = (uint)slices + 1;
            for (uint i = 0; i < stacks - 2; i++)
            {
                for (uint j = 0; j < slices; j++)
                {
                    indices.Add(baseIndex + i * ringVertexCount + j);
                    indices.Add(baseIndex + i * ringVertexCount + j + 1);
                    indices.Add(baseIndex + (i + 1) * ringVertexCount + j);

                    indices.Add(baseIndex + (i + 1) * ringVertexCount + j);
                    indices.Add(baseIndex + i * ringVertexCount + j + 1);
                    indices.Add(baseIndex + (i + 1) * ringVertexCount + j + 1);
                }
            }

            uint southPoleIndex = (uint)vertices.Count - 1;
            baseIndex = southPoleIndex - ringVertexCount;
            for (uint i = 0; i < slices; i++)
            {
                indices.Add(southPoleIndex);
                indices.Add(baseIndex + i);
                indices.Add(baseIndex + i + 1);
            }

        }

        public static void GenerateCylinder(ref List<Vertex> vertices, ref List<uint> indices, float radiusTop = 1.0f, float radiusBottom = 1.0f, float height = 1.0f, int slices = 15, int stacks = 15)
        {
            uint index = (uint)vertices.Count;

            float stackHeight = height / stacks;
            float radiusStep = (radiusTop - radiusBottom) / stacks;
            float ringCount = (float)(stacks + 1);

            for (int i = 0; i < ringCount; i++)
            {
                float y = -0.5f * height + i * stackHeight;
                float r = radiusBottom + i * radiusStep;
                float dTheta = 2.0f * (float)Math.PI / slices;

                for (int j = 0; j <= slices; j++)
                {
                    float c = (float)Math.Cos(j * dTheta);
                    float s = (float)Math.Sin(j * dTheta);

                    Vector3 v = new Vector3(r * c, y, r * s);
                    Vector2 uv = new Vector2((float)j / slices, 1.0f - (float)i / stacks);
                    Vector3 t = new Vector3(-s, 0.0f, c);

                    float dr = radiusBottom - radiusTop;
                    Vector3 bitangent = new Vector3(dr * c, -height, dr * s);

                    Vector3 n = Vector3.Cross(t, bitangent);
                    n = Vector3.Normalize(n);
                    vertices.Add(new Vertex(v, n, uv));
                }
            }

            uint ringVertexCount = (uint)slices + 1;
            for (uint i = 0; i < stacks; i++)
            {
                for (uint j = 0; j < slices; j++)
                {
                    indices.Add(index + i * ringVertexCount + j);
                    indices.Add(index + (i + 1) * ringVertexCount + j);
                    indices.Add(index + (i + 1) * ringVertexCount + j + 1);

                    indices.Add(index + i * ringVertexCount + j);
                    indices.Add(index + (i + 1) * ringVertexCount + j + 1);
                    indices.Add(index + i * ringVertexCount + j + 1);
                }
            }

            {
                // Build top cap
                uint baseIndex = (uint)vertices.Count;
                float y = 0.5f * height;
                float dTheata = 2.0f * (float)Math.PI / slices;

                Vector3 normal = new Vector3(0, 1, 0);
                Vector3 tanget = new Vector3(1, 0, 0);

                for (int i = 0; i <= slices; i++)
                {
                    float x = radiusTop * (float)Math.Cos(i * dTheata);
                    float z = radiusTop * (float)Math.Sin(i * dTheata);
                    float u = x / height + 0.5f;
                    float v = z / height + 0.5f;

                    vertices.Add(new Vertex(new Vector3(x, y, z), normal, new Vector2(u, v)));
                }

                vertices.Add(new Vertex(new Vector3(0, y, 0), normal, new Vector2()));

                uint centerIndex = (uint)vertices.Count - 1;
                for (uint i = 0; i < slices; i++)
                {
                    indices.Add(centerIndex);
                    indices.Add(baseIndex + i + 1);
                    indices.Add(baseIndex + i );
                }

                // Build bottom cap
                baseIndex = (uint)vertices.Count;
                y = -0.5f * height;

                normal = new Vector3(0, -1, 0);
                tanget = new Vector3(1, 0, 0);

                for (int i = 0; i <= slices; i++)
                {
                    float x = radiusBottom * (float)Math.Cos(i * dTheata);
                    float z = radiusBottom * (float)Math.Sin(i * dTheata);
                    float u = x / height + 0.5f;
                    float v = z / height + 0.5f;

                    vertices.Add(new Vertex(new Vector3(x, y, z), normal, new Vector2(u, v)));
                }

                vertices.Add(new Vertex(new Vector3(0, y, 0), normal, new Vector2()));

                centerIndex = (uint)vertices.Count - 1;
                for (uint i = 0; i < slices; i++)
                {
                    indices.Add(centerIndex);
                    indices.Add(baseIndex + i);
                    indices.Add(baseIndex + i + 1);
                }
            }

        }

        public static void GenerateCone(ref List<Vertex> vertices, ref List<uint> indices, float radius = 1.0f, float height = 2.0f, int slices = 15, int stacks = 15)
        {
            GenerateCylinder(ref vertices, ref indices, 0.0f, radius, height, slices, stacks);
        }
    }
}
