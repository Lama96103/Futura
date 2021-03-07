using Futura.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Resources.Import
{
    class TextureImporter : Importer
    {
        internal override AssetType AssetType => AssetType.Texture2d;

        internal override string[] SupportedExtensions => new string[] { ".png" };

        internal override Asset ImportAsset(FileInfo file)
        {
            if (!file.Exists)
            {
                Log.Error("File does not exist " + file.FullName);
                return null;
            }

            byte[] data = File.ReadAllBytes(file.FullName);

            return new Texture2D(file, Guid.NewGuid(), data);
        }
    }
}
