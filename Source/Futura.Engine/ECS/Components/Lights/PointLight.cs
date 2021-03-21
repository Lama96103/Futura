using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.ECS.Components.Lights
{
    public class PointLight : IComponent
    {
        public Color Color = Color.White;
        public float Range = 10.0f;
        
        [Range(0.0f, 1.0f, 0.05f)]
        public float Intensity = 1.0f;
    }
}
