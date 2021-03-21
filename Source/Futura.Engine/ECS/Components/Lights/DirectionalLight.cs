using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.ECS.Components.Lights
{
    public class DirectionalLight : IComponent
    {
        public Color Color;
        [Range(0.0f, 1.0f)]
        public float Intensity;
    }
}
