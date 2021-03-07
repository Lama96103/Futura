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
        private uint renderResolutionWidth = 1280;
        private uint renderResolutionHeight = 720;

        private DeviceBuffer worldBuffer;
        private DeviceBuffer modelBuffer;

        private ResourceSet worldSet;
        private ResourceSet modelSet;

        private Pipeline diffusePipline;

        private Rendering.Framebuffer diffuseFramebuffer;
        public Rendering.Framebuffer DiffuseFrameBuffer { get => diffuseFramebuffer; }

        private void Load()
        {
            ResourceFactory factory = renderAPI.Factory;

            RecreateRenderResources(1920, 1080);

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

            TransformGizmo.Init(factory);
        }

        private void RecreateRenderResources(uint width, uint height)
        {
            renderResolutionWidth = width;
            renderResolutionHeight = height;


            diffuseFramebuffer?.Unload();

            ResourceFactory factory = renderAPI.Factory;

            diffuseFramebuffer = new Rendering.Framebuffer(width, height);
            diffuseFramebuffer.Load(factory);
        }
    }
}
