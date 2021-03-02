using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Resources
{
    internal abstract class Importer
    {
        internal abstract AssetType AssetType { get; }

        internal abstract string[] SupportedExtensions { get; }

        internal abstract Asset ImportAsset(FileInfo file);
    }
}
