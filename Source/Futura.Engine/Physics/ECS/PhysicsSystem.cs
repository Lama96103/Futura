using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BepuPhysics;
using BepuPhysics.Collidables;
using Futura.Engine.ECS;
using Futura.Engine.ECS.Components;

namespace Futura.Engine.Physics.ECS
{
    class PhysicsSystem : EcsSystem
    {
        private PhysicsController physics;
        
        private EcsFilter boxCollider;
        private EcsFilter rigidBodiesFilter;

        private EcsFilter sphereCollider;

        public override void OnSetup()
        {
            boxCollider = World.CreateFilter<Transform, BoxCollider>();
            sphereCollider = World.CreateFilter<Transform, SphereCollider>();

            physics = Core.Runtime.Instance.Context.GetSubSystem<PhysicsController>();
        }

        public override void OnTick(float deltaTime)
        {
            var allColliders = boxCollider.Entities.Concat(sphereCollider.Entities);

            foreach(var reference in allColliders)
            {
                Transform transform = reference.GetComponent<Transform>();
                Collider collider = reference.Components[1] as Collider;
                collider.Tick(physics.Simulation, transform, reference.Entity.GetComponent<Rigidbody>());
            }
        }

        public override void OnEditorTick(float deltaTime)
        {
            
        }
    }
}
