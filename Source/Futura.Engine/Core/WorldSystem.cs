using Futura.ECS;
using Futura.Engine.Components;
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

        internal override void Init()
        {
            World = new EcsWorld();

            var entity = World.CreateEntity();
            entity.GetComponent<Transform>();
            var meshFilter = entity.GetComponent<MeshFilter>();
           


            entity = World.CreateEntity();
            var transform = entity.GetComponent<Transform>();
            transform.Position = new System.Numerics.Vector3(0, 0, -5);
            var camera = entity.GetComponent<Camera>();
        }

        internal override void Tick(double deltaTime)
        {
            World.Tick(deltaTime);
        }

        internal void Save(FileInfo file)
        {
            string json = Serialize.ToJson(World);
            File.WriteAllText(file.FullName, json);

        }

        internal void Load(FileInfo file)
        {
            World.Destroy();
            World = null;
            string json = File.ReadAllText(file.FullName);
            World = Serialize.ToObject<EcsWorld>(json);
            World.RefreshReferences();
        }
    }
}
