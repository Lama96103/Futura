using Futura.ECS;
using Futura.Engine.Components;
using Futura.Engine.Core;
using Futura.Engine.ECS.Components;
using Futura.Engine.Rendering;
using Futura.Engine.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.ECS.Systems
{
    class MeshGeneratorSystem : EcsSystem
    {
        private EcsFilter filter;

        private Dictionary<Entity, Mesh> Cache = new Dictionary<Entity, Mesh>();

        public override void OnSetup()
        {
            filter = World.CreateFilter<MeshFilter, MeshGenerator>();
        }

        public override void OnTick(float deltaTime)
        {
            foreach(var reference in filter.Entities)
            {
                if (reference.GetComponent<MeshGenerator>().IsDirty)
                {
                    if (!Cache.ContainsKey(reference.Entity))
                        BuildMesh(reference);
                    else
                    {
                        Mesh mesh = Cache[reference.Entity];
                        mesh.Unload();
                        Cache.Remove(reference.Entity);
                        BuildMesh(reference);
                    }
                }

                
            }
        }

        public override void OnEditorTick(float deltaTime)
        {
            OnTick(deltaTime);
        }

        private void BuildMesh(EcsFilter.EntityReference reference)
        {
            MeshFilter filter = reference.GetComponent<MeshFilter>();
            MeshGenerator gen = reference.GetComponent<MeshGenerator>();

            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            switch (gen.GeometryType)
            {
                case MeshGenerator.MeshType.None:
                    break;
                case MeshGenerator.MeshType.Quad:
                    Utility.GeometryGenerator.GenerateQuad(ref vertices, ref indices, gen.X, gen.Y);
                    break;
                case MeshGenerator.MeshType.Cube:
                    Utility.GeometryGenerator.GenerateCube(ref vertices, ref indices, gen.X, gen.Y, gen.Z);
                    break;
                case MeshGenerator.MeshType.Sphere:
                    Utility.GeometryGenerator.GenerateSphere(ref vertices, ref indices, gen.Radius, gen.Slices, gen.Stacks);
                    break;
                case MeshGenerator.MeshType.Cylinder:
                    Utility.GeometryGenerator.GenerateCylinder(ref vertices, ref indices, gen.RadiusTop, gen.RadiusBottom, gen.Height, gen.Slices, gen.Stacks);
                    break;
                case MeshGenerator.MeshType.Cone:
                    Utility.GeometryGenerator.GenerateCone(ref vertices, ref indices, gen.Radius, gen.Height);
                    break;
                default:
                    break;
            }

            gen.IsDirty = false;
            if (vertices.Count == 0) return;

            Mesh mesh = new Mesh(new System.IO.FileInfo("Mesh.mesh"), Guid.Empty, vertices.ToArray(), indices.ToArray(), new Core.Bounds());
            mesh.Load();
            filter.Mesh = mesh;
            Cache.Add(reference.Entity, mesh);

            Profiler.Report(Profiler.StatisticIndicator.BuildMesh);
        }
    }
}
