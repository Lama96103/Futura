using Futura.Engine.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Core
{
    public readonly struct Bounds : ISerialize
    {
        public Vector3 Center { get; init; }
        public Vector3 Extends { get; init; }


        /// <summary>
        /// Creates a new Bound struct
        /// </summary>
        /// <param name="v1">Can be either Center or min depending on isCenterExtends</param>
        /// <param name="v2">Can be either Extends or max depending on isCenterExtends</param>
        /// <param name="isCenterExtends">if true vector3 will directly applied if false vector3 will be taken as min/max and the bounds are calculated</param>
        public Bounds(Vector3 v1, Vector3 v2, bool isCenterExtends = true)
        {
            if (isCenterExtends)
            {
                Center = v1;
                Extends = v2;
            }
            else
            {
                Center = new Vector3((v1.X + v2.X) / 2, (v1.Y + v2.Y) / 2, (v1.Z + v2.Z) / 2);
                // TODO Test if thats correct
                Extends = new Vector3(v2.X - Center.X, v2.Y - Center.Y, v2.Z - Center.Z);
            }
        }

        public Bounds(BinaryReader reader)
        {
            Center = VectorExtension.Read(reader);
            Extends = VectorExtension.Read(reader);
        }


        public void Write(BinaryWriter writer)
        {
            Center.Write(writer);
            Extends.Write(writer);
        }

        public void Read(BinaryReader reader)
        {
            throw new NotSupportedException();
        }
    }
}
