using Futura.ECS;
using Futura.Engine.Components;
using Futura.Engine.ECS.Systems;
using Futura.Engine.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Core
{
    public class WorldSystem : SubSystem
    {
        public EcsWorld World { get; private set; }

        internal Dictionary<Type, int> WorldSystems { get; set; } = new Dictionary<Type, int>();

        public FileInfo CurrentSceneFile { get; private set; }

        internal override void Init()
        {
            World = new EcsWorld();
        }

        internal override void Tick(double deltaTime)
        {
            if (Runtime.Instance.State == Runtime.RuntimeState.Playing)
                World.Tick(Time.DeltaSeconds);
            else if (Runtime.Instance.State == Runtime.RuntimeState.Editor)
                World.EditorTick(Time.DeltaSeconds);
        }

        internal void Save()
        {
            string json = Serialize.ToJson(World);
            File.WriteAllText(CurrentSceneFile.FullName, json);
            RuntimeHelper.Instance.HasSceneChanged = false;

            string config = Serialize.ToJson(WorldSystems);
            File.WriteAllText(CurrentSceneFile.FullName + ".config.json", config);
        }

        internal void Load(FileInfo file)
        {
            CurrentSceneFile = file;

            World.Destroy();
            World = null;
            string json = File.ReadAllText(file.FullName);
            World = Serialize.ToObject<EcsWorld>(json);
            World.RefreshReferences();

            string configFile = file.FullName + ".config.json";
            if (File.Exists(configFile))
            {
                string config = File.ReadAllText(configFile);
                WorldSystems = Serialize.ToObject<Dictionary<Type, int>>(config); 

                foreach(var c in WorldSystems)
                {
                    EcsSystem sys = Activator.CreateInstance(c.Key) as EcsSystem;
                    World.RegisterSystem(sys, c.Value);
                }
            }
        }
    }
}
