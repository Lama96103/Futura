using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Resources.Import
{
    internal class MeshImporter : Importer
    {
        internal override AssetType AssetType => AssetType.Mesh;

        internal override string[] SupportedExtensions => new string[] { ".fbx", ".obj"};

        internal override Asset ImportAsset(FileInfo file)
        {
            throw new NotImplementedException();
        }
    }
}
