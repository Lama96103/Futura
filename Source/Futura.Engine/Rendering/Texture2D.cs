using Futura.Engine.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.ImageSharp;

namespace Futura.Engine.Rendering
{
    public class Texture2D : Asset
    {
        public Veldrid.Texture Handle { get; private set; } = null;
        public override bool IsLoaded => Handle != null;

        public uint Width { get => Handle.Width; }
        public uint Height { get => Handle.Height; }
        public PixelFormat Format { get => Handle.Format; }

        private byte[] data;
        public bool UseMipMap { get; set; } = true;
        public bool IsSRGB { get; set; } = true;


        private Texture2D(ResourceFactory factory, TextureDescription desc, string name) : base(Guid.Empty, AssetType.Texture2d, null)
        {
            Handle = factory.CreateTexture(desc);
            Handle.Name = name;
        }

        public Texture2D(FileInfo path, Guid guid) : base(guid, AssetType.Texture2d, path) { }
        public Texture2D(FileInfo path, Guid guid, byte[] data) : base(guid, AssetType.Texture2d, path) 
        {
            this.data = data;
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

        public override void Write(BinaryWriter writer)
        {
            writer.Write(UseMipMap);
            writer.Write(IsSRGB);
            writer.Write(data.Length);
            writer.Write(data);
        }

        public override void Read(BinaryReader reader)
        {
            UseMipMap = reader.ReadBoolean();
            IsSRGB = reader.ReadBoolean();
            int dataLength = reader.ReadInt32();
            data = reader.ReadBytes(dataLength);
        }

        public override void Load()
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                ImageSharpTexture imageSharp = new ImageSharpTexture(stream, UseMipMap, IsSRGB);
                Handle = imageSharp.CreateDeviceTexture(RenderAPI.Instance.GraphicAPI, RenderAPI.Instance.Factory);
                Handle.Name = Path.Name;
            }
        }

        public override void Unload()
        {
            RenderAPI.DisposeWhenIdle(Handle);
            Handle = null;
        }
    }
}
