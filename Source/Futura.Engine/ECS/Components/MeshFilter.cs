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
        public Mesh Mesh;
        public Material Material = new Material(new System.IO.FileInfo("C:\\"), Guid.NewGuid());
    }
}
