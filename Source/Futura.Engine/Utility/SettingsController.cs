using Futura.Engine.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Futura.Engine.Core
{
    public class SettingsController
    {
        private DirectoryInfo settingsDir;
        private string settingsNamespace;

        private readonly List<object> settings = new List<object>();

        public SettingsController(DirectoryInfo directory, string @namespace)
        {
            this.settingsDir = directory;
            this.settingsNamespace = @namespace;
            Load();
        }

        /// <summary>
        /// Loads all setting files from the folder and creates new ones if needed
        /// </summary>
        private void Load()
        {
            if (!settingsDir.Exists) settingsDir.Create();

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsClass && t.Namespace == settingsNamespace);

            foreach(Type type in types)
            {
                try
                {
                    string filePath = GetFilePath(type);
                    if (File.Exists(filePath))
                    {
                        string content = File.ReadAllText(filePath);
                        var setting = Serialize.ToObject(type, content);
                        settings.Add(setting);
                    }
                    else
                    {
                        var setting = Activator.CreateInstance(type);
                        Save(setting);
                        settings.Add(setting);
                    }
                }
                catch(Exception e)
                {
                    Log.Error("Unable to load setting " + type.Name + " , reseting to default" + type.Name, e);
                    var setting = Activator.CreateInstance(type);
                    settings.Add(setting);
                    Save(setting);
                }
                
            }
        }

        private string GetFilePath(Type type)
        {
            return Path.Combine(settingsDir.FullName, type.FullName + ".json");
        }

        public T Get<T>() where T : class
        {
            foreach(object setting in settings)
            {
                if (setting.GetType() == typeof(T)) return (T)setting;
            }
            return null;
        }

        public List<object> GetAll()
        {
            return settings;
        }

        public void Save(object setting)
        {
            string filePath = GetFilePath(setting.GetType());
            string json = Serialize.ToJson(setting);
            File.WriteAllText(filePath, json);
        }
    }
}
