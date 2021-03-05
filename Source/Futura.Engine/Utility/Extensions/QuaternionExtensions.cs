using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura
{
    public static class QuaternionExtensions
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
}
