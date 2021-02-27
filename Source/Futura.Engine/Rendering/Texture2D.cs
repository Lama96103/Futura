using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Rendering
{
    public class Texture2D
    {
        public bool IsLoaded { get => Handle != null; }
        public Veldrid.Texture Handle { get; private set; } = null;

        public uint Width { get => Handle.Width; }
        public uint Height { get => Handle.Height; }


        private Texture2D(ResourceFactory factory, TextureDescription desc, string name)
        {
            Handle = factory.CreateTexture(desc);
            Handle.Name = name;
        }



        public static Texture2D CreateRenderTarget(ResourceFactory factory, uint width, uint height, PixelFormat format, bool IsDepth = false, uint arraySize = 1, TextureUsage flags = 0, string name = "")
        {
            if (IsDepth)
            {
                var desc = new TextureDescription(width, height, 1, 1, arraySize, format, flags | TextureUsage.Sampled | TextureUsage.DepthStencil, TextureType.Texture2D);
                return new Texture2D(factory, desc, name);
            }
            else
            {
                var desc = new TextureDescription(width, height, 1, 1, arraySize, format, flags | TextureUsage.Sampled | TextureUsage.RenderTarget, TextureType.Texture2D);
                return new Texture2D(factory, desc, name);
            }
            
        }
    }
}
