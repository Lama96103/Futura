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
        public uint RenderResolutionWidth { get; private set; } = 1280;
        public uint RenderResolutionHeight { get; private set; } = 720;

        private DeviceBuffer worldBuffer;
        private DeviceBuffer modelBuffer;
        private DeviceBuffer lightingBuffer;
        private DeviceBuffer pointLightBuffer;

        private ResourceSet worldSet;
        private ResourceSet modelSet;
        private ResourceSet lightingSet;

        private Pipeline diffusePipline;
        private Pipeline gizmoPipline;
        private Pipeline wireframePipline;

        private Rendering.Framebuffer diffuseFramebuffer;
        public Rendering.Framebuffer DiffuseFrameBuffer { get => diffuseFramebuffer; }

        public Texture2D SelectionTexture { get; private set; }

        private Renderable debugSphere;
        private Renderable debugBox;

        private void Load()
        {
            ResourceFactory factory = renderAPI.Factory;

            RecreateRenderResources(1920, 1080);

            worldBuffer = factory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<WorldBuffer>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            worldBuffer.Name = "WorldBuffer";

            modelBuffer = factory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<ModelBuffer>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            modelBuffer.Name = "ModelBuffer";

            lightingBuffer = factory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<LightingBuffer>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            lightingBuffer.Name = "LightingBuffer";

            pointLightBuffer = factory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<PointLightsInfo.Blittable>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            pointLightBuffer.Name = "PointLightBuffer";

            Sampler pointSampler = renderAPI.GraphicAPI.PointSampler;

            ResourceLayout worldLayout = deviceResourceCache.GetLayout(ref deviceResourceCache.WorldLayout);
            ResourceLayout modelLayout = deviceResourceCache.GetLayout(ref deviceResourceCache.ModelLayout);
            ResourceLayout lightingLayout = deviceResourceCache.GetLayout(ref deviceResourceCache.LightingLayout);


            ResourceSetDescription wordSetDescription = new ResourceSetDescription(worldLayout, worldBuffer, pointSampler);
            worldSet = deviceResourceCache.GetSet(ref wordSetDescription);

            ResourceSetDescription modelSetDescription = new ResourceSetDescription(modelLayout, modelBuffer);
            modelSet = deviceResourceCache.GetSet(ref modelSetDescription);

            ResourceSetDescription lightingSetDescription = new ResourceSetDescription(lightingLayout, lightingBuffer, pointLightBuffer);
            lightingSet = deviceResourceCache.GetSet(ref lightingSetDescription);

            Rendering.Shader diffuseShader = new Rendering.Shader();
            diffuseShader.Compile(factory, Futura.Rendering.Resources.EditorAssets.DiffuseVertex, Futura.Rendering.Resources.EditorAssets.DiffuseFragment, true);

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription
            (
                BlendStateDescription.SingleAlphaBlend,
                DepthStencilStateDescription.DepthOnlyLessEqual,
                new RasterizerStateDescription(FaceCullMode.Back, PolygonFillMode.Solid, FrontFace.CounterClockwise, true, false),
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(new VertexLayoutDescription[] { Vertex.GetLayoutDescription() }, diffuseShader.Handles),
                new ResourceLayout[] { worldLayout, modelLayout, lightingLayout },
                diffuseFramebuffer.Handle.OutputDescription
            );

            diffusePipline = deviceResourceCache.GetPipline(ref pipelineDescription);

            pipelineDescription = new GraphicsPipelineDescription
            (
                BlendStateDescription.SingleOverrideBlend,
                DepthStencilStateDescription.Disabled,
                new RasterizerStateDescription(FaceCullMode.Back, PolygonFillMode.Solid, FrontFace.CounterClockwise, false, false),
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(new VertexLayoutDescription[] { Vertex.GetLayoutDescription() }, diffuseShader.Handles),
                new ResourceLayout[] { worldLayout, modelLayout, lightingLayout },
                diffuseFramebuffer.Handle.OutputDescription
            );

            gizmoPipline = deviceResourceCache.GetPipline(ref pipelineDescription);

            pipelineDescription = new GraphicsPipelineDescription
            (
                BlendStateDescription.SingleAlphaBlend,
                DepthStencilStateDescription.DepthOnlyLessEqual,
                new RasterizerStateDescription(FaceCullMode.None, PolygonFillMode.Wireframe, FrontFace.CounterClockwise, true, false),
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(new VertexLayoutDescription[] { Vertex.GetLayoutDescription() }, diffuseShader.Handles),
                new ResourceLayout[] { worldLayout, modelLayout, lightingLayout },
                diffuseFramebuffer.Handle.OutputDescription
            );

            wireframePipline = deviceResourceCache.GetPipline(ref pipelineDescription);

            transformGizmo.Init(factory);

            LoadDebugRenderables();
        }

        private void LoadDebugRenderables()
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();

            Utility.GeometryGenerator.GenerateSphere(ref vertices, ref indices);

            debugSphere = new Renderable();
            debugSphere.Load(renderAPI, vertices.ToArray(), indices.ToArray());

            vertices.Clear(); indices.Clear();

            Utility.GeometryGenerator.GenerateCube(ref vertices, ref indices);

            debugBox = new Renderable();
            debugBox.Load(renderAPI, vertices.ToArray(), indices.ToArray());
        }

        private void RecreateRenderResources(uint width, uint height)
        {
            RenderResolutionWidth = width;
            RenderResolutionHeight = height;

            diffuseFramebuffer?.Unload();
            SelectionTexture?.Unload();

            ResourceFactory factory = renderAPI.Factory;


            Texture2D diffuseTexture = Texture2D.CreateRenderTarget(factory, width, height, PixelFormat.R8_G8_B8_A8_UNorm, false, 1, TextureUsage.RenderTarget, "Color_RenderTarget");
            Texture2D selectionTexture = Texture2D.CreateRenderTarget(factory, width, height, PixelFormat.R8_G8_B8_A8_UNorm, false, 1, TextureUsage.RenderTarget, "Color_SelectionTarget");
            Texture2D depthTexture = Texture2D.CreateRenderTarget(factory, width, height, PixelFormat.R8_G8_B8_A8_UNorm, false, 1, TextureUsage.RenderTarget, "Color_DepthTarget");

            diffuseFramebuffer = new Rendering.Framebuffer(width, height);
            diffuseFramebuffer.Load(factory, diffuseTexture, selectionTexture, depthTexture);

            SelectionTexture = Texture2D.Create(factory, width, height, PixelFormat.R8_G8_B8_A8_UNorm, 1, TextureUsage.Staging, "Editor_Selection");
        }
    }
}
