using Futura.Engine.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura
{
    public struct Color : ISerialize
    {
        private Vector4 data;

        public float R { get => data.X; set => data.X = value; }
        public float G { get => data.Y; set => data.Y = value; }
        public float B { get => data.Z; set => data.Z = value; }
        public float A { get => data.W; set => data.W = value; }

        public Color(float r, float g, float b, float a)
        {
            data = new Vector4(r, g, b, a);
        }

        public Color(float r, float g, float b)
        {
            data = new Vector4(r, g, b, 1);
        }

        public Color(BinaryReader reader)
        {
            data = VectorExtension.ReadVector4(reader);
        }

        public static Color White { get => new Color(1, 1, 1, 1); }
        public static Color Black { get => new Color(0, 0, 0, 1); }
        public static Color Red { get => new Color(1, 0, 0, 1); }
        public static Color Green { get => new Color(0, 1, 0, 1); }
        public static Color Blue { get => new Color(0, 0, 1, 1); }

        public void Write(BinaryWriter writer)
        {
            data.Write(writer);
        }

        public void Read(BinaryReader reader)
        {
            data = VectorExtension.ReadVector4(reader);
        }
    }
}
