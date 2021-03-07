using Futura.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Resources
{
    public class Material : Asset
    {
        private bool isLoaded = true;

        public Material(FileInfo path, Guid identifier) : base(identifier, AssetType.Material, path)
        {
        }

        public override bool IsLoaded => isLoaded;

        [Name("Color")]
        public Color DiffuseColor = Color.White;
        [Name("Diffuse")]
        public Texture2D DiffuseTexture = null;


        public override void Load()
        {
        }
        public override void Unload()
        {
        }

        public override void Read(BinaryReader reader)
        {
            DiffuseColor = new Color(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            DiffuseColor.Write(writer);
        }
    }
}
