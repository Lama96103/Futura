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
        public static void Read(this Vector3 v, BinaryReader reader)
        {
            v.X = reader.ReadSingle();
            v.Y = reader.ReadSingle();
            v.Z = reader.ReadSingle();
        }
    }
}
