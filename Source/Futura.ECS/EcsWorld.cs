using Futura.ECS.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Futura.ECS
{
    public class EcsWorld
    {

        [JsonProperty] private int entityIndex = 0;

        [JsonIgnore] private int EntityIndex
        {
            get
            {
                entityIndex++;
                return entityIndex;
            }
        }

        [JsonProperty] public List<Entity> Entities { get; } = new List<Entity>();
        [JsonProperty] private List<IComponentManager> ComponentManagers = new List<IComponentManager>();

        [JsonIgnore] private List<EcsSystem> EcsSystems = new List<EcsSystem>();

        [JsonIgnore] private readonly object @lock = new object();

        public EcsWorld() { }

        #region Entity Handling
        public Entity CreateEntity()
        {
            lock (@lock)
            {
                Entity entity = new Entity(this, EntityIndex);
                Entities.Add(entity);
                return entity;
            }
        }
        public void DestroyEntity(Entity entity)
        {
            if (entity == null || !entity.IsValid) throw new EntityNotValidException();
            lock (@lock)
            {
                foreach (IComponentManager pool in ComponentManagers)
                {
                    pool.RemoveEntity(entity);
                }
                Entities.Remove(entity);
            }
        }
        public void RefreshReferences()
        {
            foreach(var entity in Entities)
            {
                entity.SetWorld(this);
            }
        }
        #endregion

        #region Component Handling
        internal T GetComponent<T> (Entity entity) where T : class, IComponent
        {
            if (!entity.IsValid) throw new EntityNotValidException();
            lock (@lock)
            {
                ComponentManager<T> pool = GetPool<T>();
                T component = pool.Get(entity);
                return component;
            }
        }
        internal void RemoveComponent<T> (Entity entity) where T : class, IComponent
        {
            if (!entity.IsValid) throw new EntityNotValidException();
            lock (@lock)
            {
                ComponentManager<T> pool = GetPool<T>();
                pool.Remove(entity);
                
            }
        }
        internal List<IComponent> GetComponents(Entity entity)
        {
            List<IComponent> components = new List<IComponent>();

            foreach(var pool in ComponentManagers)
            {
                IComponent comp = pool.GetComponent(entity);
                if (comp != null) components.Add(comp);
            }

            return components;
        }
        #endregion

        #region ComponentManager Handling
        internal ComponentManager<T> GetPool<T>() where T : class, IComponent
        {
            // First check if pool already exists
            foreach (IComponentManager pool in ComponentManagers)
            {
                if (pool.GetComponentType() == typeof(T)) return (ComponentManager<T>)pool;
            }

            // If not generate a new Componentmanager and return it
            Type poolGenericType = typeof(ComponentManager<>).MakeGenericType(new Type[] { typeof(T) });
            ComponentManager<T> poolInstance = (ComponentManager<T>)Activator.CreateInstance(poolGenericType);
            ComponentManagers.Add(poolInstance);
            return poolInstance;
        }
        #endregion

        #region EcsSystem Handling
        public T GetSystem<T>() where T : EcsSystem
        {
            foreach (EcsSystem system in EcsSystems)
                if (typeof(T) == system.GetType()) return (T)system;
            return null;
        }

        public void RegisterSystem(EcsSystem system)
        {
            EcsSystems.Add(system);
            system.Setup(this);
        }

        public void Init()
        {
            foreach (EcsSystem system in EcsSystems.OrderBy(i => i.ExecutionOrder))
                system.PreInit();
            foreach (EcsSystem system in EcsSystems.OrderBy(i => i.ExecutionOrder))
                system.Init();
        }

        public void Update()
        {
            foreach (EcsSystem system in EcsSystems.OrderBy(i => i.ExecutionOrder))
                system.Update();
        }

        public void Draw()
        {
            foreach (EcsSystem system in EcsSystems.OrderBy(i => i.ExecutionOrder))
                system.Draw();
        }
        #endregion
    }
}
