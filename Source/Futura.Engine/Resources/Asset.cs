using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Resources
{
    public enum AssetType
    {
        Unkown,
        Texture2d,
        Material,
        Mesh,
    }


    public class Asset
    {
        public Guid Identifier { get; init; }
        public AssetType AssetType { get; init; }
        public FileInfo Path { get; init; }

        internal Asset(Guid identifier, AssetType type, FileInfo path)
        {

        }

    }
}
