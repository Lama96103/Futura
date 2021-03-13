using Futura.ECS;
using Futura.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.ECS.Systems
{
    class PlayerControllerSystem : EcsSystem
    {
        private EcsFilter filter;

        public override void OnSetup()
        {
            filter = World.CreateFilter<Transform, Camera>();
        }


        public override void OnTick(float deltaTime)
        {
            float speed = 15f;

            foreach(var reference in filter.Entities)
            {
                Transform transform = reference.GetComponent<Transform>();

                if (Input.IsKey(Veldrid.Key.W))
                {
                    transform.Translate(transform.Forward() * (speed * (float)deltaTime));
                }
                if (Input.IsKey(Veldrid.Key.S))
                {
                    transform.Translate(-transform.Forward() * (speed * (float)deltaTime));
                }
                if (Input.IsKey(Veldrid.Key.A))
                {
                    transform.Translate(-transform.Right() * (speed * (float)deltaTime));
                }
                if (Input.IsKey(Veldrid.Key.D))
                {
                    transform.Translate(transform.Right() * (speed * (float)deltaTime));
                }

                Vector2 offset = Input.MouseOffset;

                //cameraRotation.Y -= offset.X * sensitivity * Time.DeltaSeconds;
                //cameraRotation.X += offset.Y * sensitivity * Time.DeltaSeconds;

                //// make sure that when pitch is out of bounds, screen doesn't get flipped
                ////if (constrainPitch)
                ////{
                ////    if (Pitch > 89.0f)
                ////        Pitch = 89.0f;
                ////    if (Pitch < -89.0f)
                ////        Pitch = -89.0f;
                ////}

                //// update Front, Right and Up Vectors using the updated Euler angles
                //Transform.Rotation = cameraRotation.FromEulerAngles();
            }
        }
    }
}
