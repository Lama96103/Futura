using Futura.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using ResourceSet = Veldrid.ResourceSet;

namespace Futura.Engine.Core
{
    partial class RenderSystem
    {
        private uint sceneWidth = 1280;
        private uint sceneHeight = 720;

        private DeviceBuffer worldBuffer;
        private DeviceBuffer modelBuffer;

        private ResourceSet worldSet;
        private ResourceSet modelSet;

        private Pipeline diffusePipline;

        private Renderable testRenderAble;

        private Rendering.Framebuffer diffuseFramebuffer;
        public Rendering.Framebuffer DiffuseFrameBuffer { get => diffuseFramebuffer; }

        private void Load()
        {
            ResourceFactory factory = renderAPI.Factory;

            Window window = Window.Instance;

            diffuseFramebuffer = new Rendering.Framebuffer(1920, 1080);
            diffuseFramebuffer.Load(factory);

            worldBuffer = factory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<WorldBuffer>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            modelBuffer = factory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<ModelBuffer>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            Sampler pointSampler = renderAPI.GraphicAPI.PointSampler;

            ResourceLayout worldLayout = deviceResourceCache.GetLayout(ref deviceResourceCache.WorldLayout);
            ResourceLayout modelLayout = deviceResourceCache.GetLayout(ref deviceResourceCache.ModelLayout);

            ResourceSetDescription wordSetDescription = new ResourceSetDescription(worldLayout, worldBuffer, pointSampler);
            worldSet = deviceResourceCache.GetSet(ref wordSetDescription);

            ResourceSetDescription modelSetDescription = new ResourceSetDescription(modelLayout, modelBuffer);
            modelSet = deviceResourceCache.GetSet(ref modelSetDescription);

            Rendering.Shader diffuseShader = new Rendering.Shader();
            diffuseShader.Compile(factory, Futura.Rendering.Resources.EditorAssets.DiffuseVertex, Futura.Rendering.Resources.EditorAssets.DiffuseFragment, true);

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription
            (
                BlendStateDescription.SingleAlphaBlend,
                DepthStencilStateDescription.DepthOnlyLessEqual,
                new RasterizerStateDescription(FaceCullMode.Back, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, false),
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(new VertexLayoutDescription[] { Vertex.GetLayoutDescription() }, diffuseShader.Handles),
                new ResourceLayout[] { worldLayout, modelLayout },
                diffuseFramebuffer.Handle.OutputDescription
            );

            diffusePipline = deviceResourceCache.GetPipline(ref pipelineDescription);



            /// -------------- TESTING ONLY
            /// 
            testRenderAble = new Renderable();
            Vertex[] vertices = new Vertex[]
            {
                new Vertex(-1, 0, 0, 0, 0, Vector3.UnitZ),
                new Vertex(-1, 1, 0, 0, 0, Vector3.UnitZ),
                new Vertex(1, 0, 0, 0, 0, Vector3.UnitZ)
            };
            uint[] indices = new uint[]
            {
                0, 1, 2
            };

            testRenderAble.Load(renderAPI, vertices, indices);
        }

        public void ResizeSceneRendering(uint width, uint height)
        {
            // diffuseFramebuffer.Resize(width, height);
            sceneWidth = width;
            sceneHeight = height;
        }
    }
}
