using Futura.ECS;
using Futura.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Components
{
    public class MeshFilter : IComponent
    {
        private Guid MaterialGuid;
        private Guid MeshGuid;


        public Renderable Mesh;
        public Material Material;
    }
}
