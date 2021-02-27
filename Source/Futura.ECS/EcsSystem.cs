using System;
using System.Collections.Generic;
using System.Text;

namespace Futura.ECS
{
    public abstract class EcsSystem
    {
        public uint ExecutionOrder { get; set; } = 100;
        protected EcsWorld World { get; private set; } = null;

        internal void Setup(EcsWorld world)
        {
            this.World = world;
            OnSetup();
        }
        internal void PreInit() { OnPreInit(); }
        internal void Init() { OnInit(); }
        internal void Update() { OnUpdate(); }
        internal void Draw() { OnDraw(); }

        protected virtual void OnSetup() { }
        protected virtual void OnPreInit() { }
        protected virtual void OnInit() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnDraw() { }
    }
}
