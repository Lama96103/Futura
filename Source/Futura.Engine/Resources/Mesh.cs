using Futura.Engine.Core;
using Futura.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Resources
{
    public class Mesh : Asset
    {
        private Vertex[] vertices;
        private uint[] indices;
        private Bounds bounds;
        private Renderable renderable = null;

        public override bool IsLoaded => renderable != null && Renderable.IsLoaded;

        public int VertexCount { get => vertices.Length; }
        public int IndexCount { get => indices.Length; }
        public Bounds Bounds { get => bounds; }
        public Renderable Renderable { get => renderable; }


        internal Mesh(FileInfo path, Guid guid) : base(guid, AssetType.Mesh, path)
        {
        }

        internal Mesh(FileInfo path, Guid guid, Vertex[] vertices, uint[] indices, Bounds bounds) : base(guid, AssetType.Mesh, path)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.bounds = bounds;
        }


        public override void Write(BinaryWriter writer)
        {
            writer.Write(vertices.Length);
            foreach (Vertex v in vertices) v.Write(writer);
            writer.Write(indices.Length);
            foreach (uint i in indices) writer.Write(i);
            bounds.Write(writer);
        }

        public override void Read(BinaryReader reader)
        {
            int vertexLength = reader.ReadInt32();
            vertices = new Vertex[vertexLength];
            for (int i = 0; i < vertexLength; i++) vertices[i] = new Vertex(reader);
            
            int indexLength = reader.ReadInt32();
            indices = new uint[indexLength];
            for (int i = 0; i < indexLength; i++) indices[i] = reader.ReadUInt32();
            bounds = new Bounds(reader);

            if (bounds == default(Bounds)) RecaculateBounds();
        }

        public void RecaculateBounds()
        {
            Vector3 boundsMin = new Vector3();
            Vector3 boundsMax = new Vector3();

            foreach(Vertex v in vertices)
            {
                boundsMin = Vector3.Min(boundsMin, v.Position);
                boundsMax = Vector3.Max(boundsMax, v.Position);
            }

            bounds = new Bounds(boundsMin, boundsMax, false);

            HasAssetChanged = true;
        }

        public override void Load()
        {
            if (IsLoaded) Log.Error("Mesh is already loaded");
            renderable = new Renderable();
            renderable.Load(Runtime.Instance.Context.GetSubSystem<RenderSystem>().API, vertices, indices);
            Profiler.Report(Profiler.StatisticIndicator.Load_Mesh);
        }

        public override void Unload()
        {
            if (!IsLoaded) Log.Error("Mesh is already unloaded");
            renderable?.Unload();
        }
    }
}
