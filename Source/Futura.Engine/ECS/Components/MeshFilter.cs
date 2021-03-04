using Futura.ECS;
using Futura.Engine.Rendering;
using Futura.Engine.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Components
{
    public class MeshFilter : IComponent
    {
        [Asset(AssetType.Material)]
        private Guid MaterialGuid;

        [Asset(AssetType.Mesh)]
        private Guid MeshGuid;


        public Renderable Mesh;
        public Material Material;
    }

    public class AssetAttribute : Attribute
    {
        public AssetType Asset { get; init; }

        public AssetAttribute(AssetType assetType)
        {
            Asset = assetType;
        }
    }
}
