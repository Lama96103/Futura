using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Rendering
{
    public class Framebuffer
    {
        public uint Width { get; private set; }
        public uint Height { get; private set; }

        public Texture2D[] ColorTextures { get; private set; }
        public Texture2D DepthTexture { get; private set; }

        internal Veldrid.Framebuffer Handle { get; private set; } = null;

        public bool IsLoaded { get => Handle != null; }

        public Framebuffer(uint width, uint height)
        {
            this.Width = width;
            this.Height = height;
        }

        internal void Load(ResourceFactory factory)
        {
            var colorTexture = Texture2D.CreateRenderTarget(factory, Width, Height, PixelFormat.R8_G8_B8_A8_UNorm, false, 1, TextureUsage.RenderTarget, "Color_RenderTarget");
            ColorTextures = new Texture2D[] { colorTexture };

            DepthTexture = Texture2D.CreateRenderTarget(factory, Width, Height, PixelFormat.D32_Float_S8_UInt, true, 1, TextureUsage.DepthStencil, "Depth_RenderTarget");

            Handle = factory.CreateFramebuffer(new FramebufferDescription(DepthTexture.Handle, colorTexture.Handle));
        }

        internal void Load(ResourceFactory factory, params Texture2D[] colorTargets)
        {
            DepthTexture = Texture2D.CreateRenderTarget(factory, Width, Height, PixelFormat.D32_Float_S8_UInt, true, 1, TextureUsage.DepthStencil, "Depth_RenderTarget");
            ColorTextures = colorTargets;

            Texture[] handles = new Texture[ColorTextures.Length];
            for (int i = 0; i < handles.Length; i++)
            {
                handles[i] = ColorTextures[i].Handle;
            }


            Handle = factory.CreateFramebuffer(new FramebufferDescription(DepthTexture.Handle, handles));
        }

        internal void Unload()
        {
            foreach (Texture2D colorTexture in ColorTextures) colorTexture.Unload();
            DepthTexture.Unload();
            RenderAPI.DisposeWhenIdle(Handle);

            Handle = null;
            ColorTextures = null;
            DepthTexture = null;
        }
    }
}
