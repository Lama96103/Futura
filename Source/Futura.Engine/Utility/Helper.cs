using Futura.Engine.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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

        public static string CaculateHash(FileInfo file)
        {
            using(MD5 md5Hash = MD5.Create())
            {
                byte[] data = File.ReadAllBytes(file.FullName);
                byte[] hash = md5Hash.ComputeHash(data);

                var sBuilder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sBuilder.Append(hash[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }


        public const int HashLength = 16;
        public static byte[] CacluateHash(FileInfo file)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = File.ReadAllBytes(file.FullName);
                return md5Hash.ComputeHash(data);
            }
        }
    }
}
