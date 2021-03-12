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

        #region Vector2
        /// <summary>
        ///  Writes the object to the binary writer
        /// </summary>
        /// <param name="v"></param>
        /// <param name="writer"></param>
        public static void Write(this Vector2 v, BinaryWriter writer)
        {
            writer.Write(v.X);
            writer.Write(v.Y);
        }


        /// <summary>
        /// Reads the reader and sets the values
        /// </summary>
        /// <param name="v"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Vector2 ReadVector2(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            return new Vector2(x, y);
        }
        #endregion

        #region Vector3
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
        public static Vector3 ReadVector3(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }
        #endregion

        #region Vector4
        /// <summary>
        ///  Writes the object to the binary writer
        /// </summary>
        /// <param name="v"></param>
        /// <param name="writer"></param>
        public static void Write(this Vector4 v, BinaryWriter writer)
        {
            writer.Write(v.X);
            writer.Write(v.Y);
            writer.Write(v.Z);
            writer.Write(v.W);
        }


        /// <summary>
        /// Reads the reader and sets the values
        /// </summary>
        /// <param name="v"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Vector4 ReadVector4(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float w = reader.ReadSingle();
            return new Vector4(x, y, z, w);
        }
        #endregion
    }
}
