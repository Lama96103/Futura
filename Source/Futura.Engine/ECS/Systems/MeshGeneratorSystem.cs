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

        public override void OnTick(double deltaTime)
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
                    Utility.GeometryGenerator.GenerateQuad(ref vertices, ref indices, gen.Width, gen.Depth);
                    break;
                case MeshGenerator.MeshType.Cube:
                    break;
                default:
                    break;
            }

            if (vertices.Count == 0) return;

            Mesh mesh = new Mesh(new System.IO.FileInfo("Mesh.mesh"), Guid.Empty, vertices.ToArray(), indices.ToArray(), new Core.Bounds());
            mesh.Load();
            filter.Mesh = mesh;
            Cache.Add(reference.Entity, mesh);

            gen.IsDirty = false;
            Profiler.Report(Profiler.StatisticIndicator.BuildMesh);
        }
    }
}
