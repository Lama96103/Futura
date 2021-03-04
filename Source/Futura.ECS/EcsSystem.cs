using System;
using System.Collections.Generic;
using System.Text;

namespace Futura.ECS
{
    public abstract class EcsSystem
    {
        public uint ExecutionOrder { get; private set; } = 100;
        protected EcsWorld World { get; private set; } = null;

        internal void Setup(EcsWorld world, uint execOrder)
        {
            this.World = world;
            this.ExecutionOrder = execOrder;
            OnSetup();
        }

        public virtual void OnSetup() { }
        public virtual void OnInit() { }
        public virtual void OnTick(double deltaTime) { }
    }
}
