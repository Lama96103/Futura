using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Futura.Engine
{
    public static class MathExtensions
    {
        public static double ToRadians(this double val)
        {
            /// <summary>
            /// Convert to Radians.
            /// </summary>
            /// <param name="val">The value to convert to radians</param>
            /// <returns>The value in radians</returns>
            return (System.Math.PI / 180) * val;
        }

        public static float ToRadians(this float val)
        {
            /// <summary>
            /// Convert to Radians.
            /// </summary>
            /// <param name="val">The value to convert to radians</param>
            /// <returns>The value in radians</returns>
            return ((float)System.Math.PI / 180.0f) * val;
        }

        public static double ToDegree(this double val)
        {
            /// <summary>
            /// Convert to Degree.
            /// </summary>
            /// <param name="val">The value to convert to degree</param>
            /// <returns>The value in degree</returns>
            return (180/System.Math.PI) * val;
        }

        public static float ToDegree(this float val)
        {
            /// <summary>
            /// Convert to Degree.
            /// </summary>
            /// <param name="val">The value to convert to degree</param>
            /// <returns>The value in degree</returns>
            return ((float)(180 / System.Math.PI)) * val;
        }
    }


    public static class QuaternionExtension
    {
        public static Vector3 Forward(this Quaternion val)
        {
            return Vector3.Transform(VectorExtension.Forward, val);
        }

        public static Vector3 Right(this Quaternion val)
        {
            return Vector3.Transform(VectorExtension.Right, val);
        }

        public static Vector3 Up(this Quaternion val)
        {
            return Vector3.Transform(VectorExtension.Up, val);
        }

        public static bool Smaller(this Vector3 v1, Vector3 v2)
        {
            return v1.X < v2.X || v1.Y < v2.Y || v1.Z < v2.Z;
        }
    }

    public static class MatrixExtension
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

    public static class MathHelper
    {
        public static int GetIndex(Vector3 bounds, int x, int y, int z)
        {
            return (int)(x + (y * bounds.X) + (z * bounds.X * bounds.Y));
        }
    }
}
