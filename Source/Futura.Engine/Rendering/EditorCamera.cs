using Futura.Engine.Components;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Rendering
{
    public class EditorCamera : Singleton<EditorCamera>
    {
        public Transform Transform { get; set; } 
            = new Transform()
            {
                Position = new System.Numerics.Vector3(0, 0, -10),
                Rotation = Quaternion.Identity
            };

        public Camera Camera { get; set; } = new Camera();


        private float speed = 8f;
        private float sensitivity = 32f;

        private Vector3 cameraRotation = new Vector3();

        public void Tick()
        {

            ProcessTranslation();
            ProcessRotation();
        }

        private void ProcessTranslation()
        {
            float veloctiy = speed * Time.DeltaSeconds;

            if (ImGui.IsKeyDown((int)Key.W))
            {
                Transform.Position += (Transform.Forward() * veloctiy);
            }

            if (ImGui.IsKeyDown((int)Key.S))
            {
                Transform.Position -= (Transform.Forward() * veloctiy);
            }

            if (ImGui.IsKeyDown((int)Key.A))
            {
                Transform.Position += (Transform.Right() * veloctiy);
            }

            if (ImGui.IsKeyDown((int)Key.D))
            {
                Transform.Position -= (Transform.Right() * veloctiy);
            }

            if (ImGui.IsKeyDown((int)Key.Q))
            {
                Transform.Position -= (Transform.Up() * veloctiy);
            }
            if (ImGui.IsKeyDown((int)Key.E))
            {
                Transform.Position += (Transform.Up() * veloctiy);
            }
        }

        private void ProcessRotation()
        {
            Vector2 offset = Input.MouseOffset;

            cameraRotation.Y -= offset.X * sensitivity * Time.DeltaSeconds;
            cameraRotation.X += offset.Y * sensitivity * Time.DeltaSeconds;

            // make sure that when pitch is out of bounds, screen doesn't get flipped
            //if (constrainPitch)
            //{
            //    if (Pitch > 89.0f)
            //        Pitch = 89.0f;
            //    if (Pitch < -89.0f)
            //        Pitch = -89.0f;
            //}

            // update Front, Right and Up Vectors using the updated Euler angles
            Transform.Rotation = cameraRotation.FromEulerAngles();
        }

    }
}
