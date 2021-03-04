using Futura.Engine.Utility.CustomSerializer;
using Newtonsoft.Json;
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
        #region Serialize Json
        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, GetJsonSettings());
        }

        public static T ToObject<T>(string json)
        {
            return (T)(JsonConvert.DeserializeObject(json, typeof(T), GetJsonSettings()));
        }

        public static object ToObject(Type type, string json)
        {
            return JsonConvert.DeserializeObject(json, type, GetJsonSettings());
        }

        private static JsonSerializerSettings GetJsonSettings()
        {
            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                TypeNameHandling = TypeNameHandling.All,
                Converters = new JsonConverter[] 
                { 
                    new AssetSerializer<Resources.Mesh>()
                },
                CheckAdditionalContent = true
            };
            setting.Error += OnErrorHandler;
            return setting;
        }

        private static void OnErrorHandler(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
        {
            Log.Error("Could not serialize " + args.CurrentObject.ToString());
            args.ErrorContext.Handled = true;
        }

    
        #endregion
    }

    public interface ISerialize
    {
        public void Write(BinaryWriter writer);
        public void Read(BinaryReader reader);
    }
}
