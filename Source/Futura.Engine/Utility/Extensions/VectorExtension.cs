using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura
{
    public static class VectorExtension
    {
        public static Vector3 Forward { get { return new Vector3(1.0f, 0.0f, 0.0f); } }
        public static Vector3 Right { get { return new Vector3(0.0f, 0.0f, 1.0f); } }
        public static Vector3 Up { get { return new Vector3(0.0f, 1.0f, 0.0f); } }


        /// <summary>
        ///  Writes the object to the binary writer
        /// </summary>
        /// <param name="v"></param>
        /// <param name="writer"></param>
        public static void Write(this Vector3 v, BinaryWriter writer)
        {
            writer.Write(v.X);
            writer.Write(v.Y);
            writer.Write(v.Z);
        }


        /// <summary>
        /// Reads the reader and sets the values
        /// </summary>
        /// <param name="v"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Vector3 Read(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }
    }
}
