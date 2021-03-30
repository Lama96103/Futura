using BepuPhysics;
using BepuPhysics.Collidables;
using Futura.Engine.ECS;
using Futura.Engine.ECS.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Physics.ECS
{
    abstract class Collider : IComponent, IComponentChangeListener
    {
        public Vector3 Offset = Vector3.Zero;
        [JsonIgnore] [Ignore] public BodyReference Reference;

        private bool hasChanged = true;

        internal void Tick(Simulation simulation, Transform transform, Rigidbody rigidbody)
        {
            if (hasChanged)
            {
                if (Reference.Exists && Reference.Kinematic == (rigidbody == null))
                {
                    Resize();   
                }
                else
                {
                    if (Reference.Exists)
                    {
                        simulation.Bodies.Remove(Reference.Handle);
                        simulation.Shapes.Remove(Reference.Collidable.Shape);
                    }

                    TypedIndex index = CreateCollisionShape(simulation, (rigidbody != null ? rigidbody.Mass : 0), out BodyInertia inertia);

                    var collidableDescription = new CollidableDescription(index, 0.1f);

                    var pose = new RigidPose(transform.Position + Offset, transform.Rotation);

                    BodyHandle handle;

                    if ((rigidbody == null))
                        handle = simulation.Bodies.Add(BodyDescription.CreateKinematic(pose, collidableDescription, new BodyActivityDescription(0.01f)));
                    else
                        handle = simulation.Bodies.Add(BodyDescription.CreateDynamic(pose, inertia, collidableDescription, new BodyActivityDescription(0.01f)));

                    Reference = new BodyReference(handle, simulation.Bodies);

                }

                hasChanged = false;
            }

            if (rigidbody != null)
            {
                ref RigidPose rigidPose = ref Reference.Pose;
                transform.Position = rigidPose.Position;
                transform.Rotation = rigidPose.Orientation;
            }
            else
            {
                ref RigidPose rigidPose = ref Reference.Pose;
                rigidPose.Position = transform.Position;
                rigidPose.Orientation = transform.Rotation;
            }
        }

        protected abstract void Resize();

        protected abstract TypedIndex CreateCollisionShape(Simulation simulation, float mass, out BodyInertia inertia);

        public void OnChanged()
        {
            hasChanged = true;
        }
    }

    class BoxCollider : Collider
    {
        public Vector3 Size = Vector3.One;
        
        [JsonIgnore][Ignore] public Box CollisionBox;

        protected override TypedIndex CreateCollisionShape(Simulation simulation, float mass, out BodyInertia inertia)
        {
            CollisionBox = new Box(Size.X * 2, Size.Y * 2, Size.Z * 2);
            CollisionBox.ComputeInertia(mass, out inertia);
            return simulation.Shapes.Add(CollisionBox);
        }

        protected override void Resize()
        {
            CollisionBox.Width = Size.X * 2;
            CollisionBox.Height = Size.Y * 2;
            CollisionBox.Length = Size.Z * 2;
        }
    }

    class SphereCollider : Collider
    {
        public float Radius = 1.0f;

        [JsonIgnore] [Ignore] public Sphere CollisionSphere;

        protected override void Resize()
        {
            CollisionSphere.Radius = Radius;
        }

        protected override TypedIndex CreateCollisionShape(Simulation simulation, float mass, out BodyInertia inertia)
        {
            CollisionSphere = new Sphere(Radius);
            CollisionSphere.ComputeInertia(mass, out inertia);
            return simulation.Shapes.Add(CollisionSphere);
        }
    }
}
