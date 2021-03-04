using Futura.ECS;
using Futura.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Core
{
    public class WorldSystem : SubSystem
    {
        public EcsWorld World { get; private set; }

        internal override void Init()
        {
            World = new EcsWorld();

            var entity = World.CreateEntity();
            entity.GetComponent<Transform>();
            var meshFilter = entity.GetComponent<MeshFilter>();
           


            entity = World.CreateEntity();
            var transform = entity.GetComponent<Transform>();
            transform.Position = new System.Numerics.Vector3(0, 0, -5);
            var camera = entity.GetComponent<Camera>();
        }

        internal override void Tick(double deltaTime)
        {
            World.Tick(deltaTime);
        }
    }
}
