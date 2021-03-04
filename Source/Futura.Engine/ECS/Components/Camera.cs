using Futura.ECS;
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
        // Optimize for only one calculation per frame

        public ProjectionType Projection = ProjectionType.Perspective;
        public float FieldOfView = 80f;

        private float nearPlane = 0.3f;
        private float farPlane = 1000.0f;


        public Matrix4x4 GetView(Transform transform)
        {
            var position = transform.Position;
            var lookAt = Vector3.Transform(Vector3.UnitZ, transform.Rotation);
            var up = Vector3.Transform(Vector3.UnitY, transform.Rotation);

            lookAt += position;

            return Matrix4x4.CreateLookAt(position, lookAt, up);
        }

        public Matrix4x4 GetProjection(Transform transform, uint width, uint height)
        {
            if(Projection == ProjectionType.Perspective)
            {
                return Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView.ToRadians(), (float)width / (float)height, nearPlane, farPlane);
            }
            else
            {
                return Matrix4x4.CreateOrthographic((float)width, (float)height, nearPlane, farPlane);
            }
        }

        public Matrix4x4 GetViewProjectionMatrix(Transform transform, uint width, uint height)
        {
            return GetView(transform) * GetProjection(transform, width, height);
        }
    }
}
