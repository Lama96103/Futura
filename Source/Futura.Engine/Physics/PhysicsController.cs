using BepuPhysics;
using BepuUtilities.Memory;
using Futura.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Physics
{
    class PhysicsController : SubSystem
    {
        public Simulation Simulation { get; private set; }
        private BufferPool bufferPool;

        internal override void Init()
        {
            bufferPool = new BufferPool();
            Simulation = Simulation.Create(bufferPool, new NarrowPhaseCallbacks(), new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -9.81f, 0)), new PositionLastTimestepper());
        }


        internal override void Tick(double deltaTime)
        {
            Simulation.Timestep(Time.DeltaSeconds);
        }

    }
}
