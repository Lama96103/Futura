using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Utility
{
    public static class Serialize
    {
       
    }

    public interface ISerialize
    {
        public void Write(BinaryWriter writer);
        public void Read(BinaryReader reader);
        
    }
}
