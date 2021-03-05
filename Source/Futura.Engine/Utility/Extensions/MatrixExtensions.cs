using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura
{
    public static class MatrixExtensions
    {
        public static Matrix4x4 CalculateModelMatrix(in Vector3 position, in Quaternion rotation, in Vector3 scale)
        {
            Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(position);
            Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(rotation);
            Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scale);
            return scaleMatrix * rotationMatrix * translationMatrix;
        }

        public static Matrix4x4 CalculateModelMatrix(in Vector2 position, in Quaternion rotation, in Vector2 scale)
        {
            Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(new Vector3(position.X, position.Y, 0));
            Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(rotation);
            Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(new Vector3(scale.X, scale.Y, 1));
            return scaleMatrix * rotationMatrix * translationMatrix;
        }
    }
}
