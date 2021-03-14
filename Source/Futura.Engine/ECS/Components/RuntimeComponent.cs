using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.ECS
{
    [Ignore]
    class RuntimeComponent : IComponent
    {
        public string Name = "Entity";
        public bool IsEnabled = true;
        public bool IsSelected = false;
    }
}
