using Futura.Engine.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Futura.Engine
{
    public static class Helper
    {
        public static byte[] GetEmbeddedRessource(string ressourceName, Assembly assembly = null)
        {
            if(assembly == null)
                assembly = Assembly.GetExecutingAssembly();

#if DEBUG
            if (!GetEmbeddedRessourceNames(assembly).Contains(ressourceName))
            {
                Log.Error("Ressource " + ressourceName + " does not exists in " + assembly.FullName);
                return new byte[0];
            }
#endif
            using(Stream stream = assembly.GetManifestResourceStream(ressourceName))
            {
                byte[] value = new byte[stream.Length];
                stream.Read(value, 0, (int)stream.Length);
                return value;
            }
        }

        public static string[] GetEmbeddedRessourceNames(Assembly assembly)
        {
            return assembly.GetManifestResourceNames();
        }
    }
}
