using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura
{
    public static class MathExtensions
    {
        /// <summary>
        /// Convert to Radians.
        /// </summary>
        /// <param name="val">The value to convert to radians</param>
        /// <returns>The value in radians</returns>
        public static double ToRadians(this double val)
        {
            return (System.Math.PI / 180) * val;
        }

        /// <summary>
        /// Convert to Radians.
        /// </summary>
        /// <param name="val">The value to convert to radians</param>
        /// <returns>The value in radians</returns>
        public static float ToRadians(this float val)
        {
            return ((float)System.Math.PI / 180.0f) * val;
        }

        /// <summary>
        /// Convert to Degree.
        /// </summary>
        /// <param name="val">The value to convert to degree</param>
        /// <returns>The value in degree</returns>
        public static double ToDegree(this double val)
        {
            return (180 / System.Math.PI) * val;
        }

        /// <summary>
        /// Convert to Degree.
        /// </summary>
        /// <param name="val">The value to convert to degree</param>
        /// <returns>The value in degree</returns>
        public static float ToDegree(this float val)
        {
            return ((float)(180 / System.Math.PI)) * val;
        }

        public static int GetIndex(Vector3 bounds, int x, int y, int z)
        {
            return (int)(x + (y * bounds.X) + (z * bounds.X * bounds.Y));
        }
    }
}
