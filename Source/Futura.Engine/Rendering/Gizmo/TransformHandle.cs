using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Rendering.Gizmo
{
    abstract class TransformHandle
    {
        public abstract void Init(Veldrid.ResourceFactory factory);

        public void Tick(CommandList commandList)
        {

        }
    }
}
