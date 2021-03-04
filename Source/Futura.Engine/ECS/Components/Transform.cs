using Futura.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Components
{
    public class Transform : IComponent
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Quaternion Rotation = Quaternion.Identity;

        public Matrix4x4 CalculateModelMatrix()
        {
            return CalculateModelMatrix(Position, Rotation, Scale);
        }

        public Vector3 Forward()
        {
            return Vector3.Transform(Vector3.UnitX, Rotation);
        }

        public Vector3 Right()
        {
            return Vector3.Transform(Vector3.UnitZ, Rotation);
        }

        public Vector3 Up()
        {
            return Vector3.Transform(Vector3.UnitY, Rotation);
        }


        #region Static Functions
        public static Matrix4x4 CalculateModelMatrix(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return Matrix4x4.CreateScale(scale) * Matrix4x4.CreateFromQuaternion(rotation) * Matrix4x4.CreateTranslation(position);
        }
        #endregion
    }
}
