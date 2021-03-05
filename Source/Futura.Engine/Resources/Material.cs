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

        public Color DiffuseColor = Color.White;


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
