using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Core
{
    public class InputSystem : SubSystem
    {
        internal InputSnapshot Snapshot { get; private set; }

        internal override void Tick(double deltaTime)
        {
            Snapshot =  Window.Instance.PumpEvents();
        }
    }
}
