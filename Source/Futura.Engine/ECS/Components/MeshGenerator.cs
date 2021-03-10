using Futura.ECS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.ECS.Components
{
    class MeshGenerator : IComponent
    {
        public float Width = 1;
        public float Depth = 1;
        public MeshType GeometryType = MeshType.None;

        [JsonIgnore] public bool IsDirty = true;

        public enum MeshType { None, Quad, Cube }
    }
}
