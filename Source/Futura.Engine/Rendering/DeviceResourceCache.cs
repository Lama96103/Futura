using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;

namespace Futura.Engine.Rendering
{
    internal class DeviceResourceCache
    {
        #region Base ResourceLayoutDescriptions
        public ResourceLayoutDescription WorldLayout = new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
            new ResourceLayoutElementDescription("TextureSampler", ResourceKind.Sampler, ShaderStages.Fragment));


        public ResourceLayoutDescription ModelLayout = new ResourceLayoutDescription(
           new ResourceLayoutElementDescription("ModelBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment));
        #endregion

        private ResourceFactory factory;

        internal DeviceResourceCache(ResourceFactory factory)
        {
            this.factory = factory;
        }


        private readonly Dictionary<ResourceLayoutDescription, ResourceLayout> resourceLayouts = 
            new Dictionary<ResourceLayoutDescription, ResourceLayout>();

        private readonly Dictionary<ResourceSetDescription, ResourceSet> resourceSets =
          new Dictionary<ResourceSetDescription, ResourceSet>();

        private readonly Dictionary<GraphicsPipelineDescription, Pipeline> piplines =
            new Dictionary<GraphicsPipelineDescription, Pipeline>();

        private readonly Dictionary<Texture, TextureView> textureViews =
            new Dictionary<Texture, TextureView>();


        public Pipeline GetPipline(ref GraphicsPipelineDescription desc)
        {
            if(!piplines.TryGetValue(desc, out Pipeline pipline))
            {
                pipline = factory.CreateGraphicsPipeline(ref desc);
                piplines.Add(desc, pipline);
            }
            return pipline;
        }

        public ResourceLayout GetLayout(ref ResourceLayoutDescription desc)
        {
            if(!resourceLayouts.TryGetValue(desc, out ResourceLayout layout))
            {
                layout = factory.CreateResourceLayout(ref desc);
                resourceLayouts.Add(desc, layout);
            }
            return layout;
        }

        public ResourceSet GetSet(ref ResourceSetDescription desc)
        {
            if (!resourceSets.TryGetValue(desc, out ResourceSet set))
            {
                set = factory.CreateResourceSet(ref desc);
                resourceSets.Add(desc, set);
            }
            return set;
        }

        public TextureView GetTextureView(Texture texture)
        {
            if(!textureViews.TryGetValue(texture, out TextureView view))
            {
                view = factory.CreateTextureView(texture);
                textureViews.Add(texture, view);
            }
            return view;
        }

    }
}
