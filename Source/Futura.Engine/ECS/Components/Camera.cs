using Futura.ECS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Components
{
    public enum ProjectionType
    {
        Perspective, Orthographic
    }

    public class Camera : IComponent
    {
        public ProjectionType Projection = ProjectionType.Perspective;
        public float FieldOfView = 80f;

        public float NearPlane = 0.3f;
        public float FarPlane = 1000.0f;

        bool IsMainRenderCamera = true;

        [JsonIgnore][Ignore]
        public Matrix4x4 ViewMatrix { get; private set; }

        [JsonIgnore][Ignore]
        public Matrix4x4 ProjectionMatrix { get; private set; }

        [JsonIgnore][Ignore]
        public Matrix4x4 ViewProjectionMatrix { get; private set; }


        public void UpdatePosition(Transform transform, float width, float height)
        {
            Vector3 position = transform.Position;
            Vector3 lookAt = Vector3.Transform(Vector3.UnitZ, transform.Rotation);
            lookAt += position;
            Vector3 up = Vector3.Transform(Vector3.UnitY, transform.Rotation);

            ViewMatrix = Matrix4x4.CreateLookAt(position, lookAt, up);

            if (NearPlane <= 0) NearPlane = 0.1f;
            if (FarPlane <= 0) FarPlane = 0.1f;
            if (NearPlane >= FarPlane) FarPlane += 0.1f;

            if (Projection == ProjectionType.Perspective)
            {
                if (FieldOfView.ToRadians() <= 0) FieldOfView = 0.1f.ToDegree();
                if (FieldOfView.ToRadians() >= Math.PI) FieldOfView = ((float)Math.PI - 0.1f).ToDegree();

                ProjectionMatrix =  Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView.ToRadians(), (float)width / (float)height, NearPlane, FarPlane);
            }
            else
            {
                ProjectionMatrix = Matrix4x4.CreateOrthographic((float)width, (float)height, NearPlane, FarPlane);
            }

            ViewProjectionMatrix = ViewMatrix * ProjectionMatrix;
        }
    }
}
