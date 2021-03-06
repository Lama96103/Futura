using Futura.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Rendering
{
    public class Renderable
    {
        public bool IsLoaded { get; private set; } = false;

        private DeviceBuffer vertexBuffer = null;
        private DeviceBuffer indexBuffer = null;

        private uint vertexCount = 0;
        private uint indexCount = 0;

        public uint Vertices { get => vertexCount; }
        public uint Indices { get => indexCount; }

        /// <summary>
        /// Loads the mesh into the Graphics Device
        /// </summary>
        /// <param name="verticies"></param>
        /// <param name="indicies"></param>
        public void Load(RenderAPI api, Vertex[] verticies, uint[] indicies)
        {
            if (IsLoaded)
            {
                Log.Warn("Mesh is already loaded");
                return;
            }

            vertexCount = (uint)verticies.Length;
            indexCount = (uint)indicies.Length;


            vertexBuffer = api.Factory.CreateBuffer(new BufferDescription(vertexCount * Vertex.Size, BufferUsage.VertexBuffer));
            indexBuffer = api.Factory.CreateBuffer(new BufferDescription(indexCount * sizeof(uint), BufferUsage.IndexBuffer));

            api.GraphicAPI.UpdateBuffer(vertexBuffer, 0, verticies);
            api.GraphicAPI.UpdateBuffer(indexBuffer, 0, indicies);

            IsLoaded = true;
        }

        public void Unload()
        {
            RenderAPI.DisposeWhenIdle(vertexBuffer);
            RenderAPI.DisposeWhenIdle(indexBuffer);
            vertexBuffer = null;
            indexBuffer = null;

            IsLoaded = false;
        }

        /// <summary>
        /// Draws Mesh
        /// </summary>
        /// <param name="commandList"></param>
        internal void Draw(CommandList commandList)
        {
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt32);

            commandList.DrawIndexed(
                indexCount: indexCount,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);

            Profiler.Report(Profiler.StatisticIndicator.DrawCall);
            Profiler.Report(Profiler.StatisticIndicator.Vertex, (int)vertexCount);

        }
    }
}
