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


        public static Vector3 ToEulerAngles(this Quaternion q)
        {
            float check = 2.0f * (-q.Y * q.Z + q.W * q.X);

            if(check < -0.995f)
            {
                return new Vector3(-90.0f, 0.0f, -((float)Math.Atan2(2.0f * (q.X * q.Z - q.W * q.Y), 1.0f - 2.0f * (q.Y * q.Y + q.Z * q.Z))).ToDegree());
            }

            if (check > 0.995f)
            {
                return new Vector3(90.0f, 0.0f, -((float)Math.Atan2(2.0f * (q.X * q.Z - q.W * q.Y), 1.0f - 2.0f * (q.Y * q.Y + q.Z * q.Z))).ToDegree());
            }

            return new Vector3(
                ((float)Math.Asin(check)).ToDegree(),
                ((float)Math.Atan2(2.0f * (q.X * q.Z + q.W * q.Y), 1.0f - 2.0f * (q.X * q.X + q.Y * q.Y))).ToDegree(),
                ((float)Math.Atan2(2.0f * (q.X *q.Y + q.W * q.Z), 1.0f - 2.0f * (q.X * q.X + q.Z * q.Z))).ToDegree()
                );
        }

        public static Quaternion FromEulerAngles(this Vector3 vector)
        {
            return Quaternion.CreateFromYawPitchRoll(vector.Y.ToRadians(), vector.X.ToRadians(), vector.Z.ToRadians());
        }
    }
}
