using Futura.Engine.ECS.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Futura.Engine.ECS
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
        [JsonIgnore] private List<EcsFilter> cachedFilters = new List<EcsFilter>();

        [JsonIgnore] private readonly object @lock = new object();


        public EcsWorld() { }

        #region Entity Handling
        /// <summary>
        /// Creates a new empty entity
        /// </summary>
        /// <returns></returns>
        public Entity CreateEntity()
        {
            lock (@lock)
            {
                Entity entity = new Entity(this, EntityIndex);
                Entities.Add(entity);
                return entity;
            }
        }
        /// <summary>
        /// Destorys the entity and delete all its components
        /// </summary>
        /// <param name="entity"></param>
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
        /// <summary>
        /// Sets for all entities this world
        /// </summary>
        public void RefreshReferences()
        {
            foreach(var entity in Entities)
            {
                entity.SetWorld(this);
            }
        }
        #endregion

        #region Component Handling
        internal T Get<T> (Entity entity) where T : class, IComponent
        {
            if (!entity.IsValid) throw new EntityNotValidException();
            lock (@lock)
            {
                ComponentManager<T> pool = GetPool<T>();
                T component = pool.Get(entity);
                return component;
            }
        }
        internal T GetComponent<T>(Entity entity) where T : class, IComponent
        {
            if (!entity.IsValid) throw new EntityNotValidException();
            lock (@lock)
            {
                ComponentManager<T> pool = GetPool<T>();
                T component = pool.GetComponent(entity);
                return component;
            }
        }
        internal bool HasComponent<T>(Entity entity) where T : class, IComponent
        {
            if (!entity.IsValid) throw new EntityNotValidException();
            lock (@lock)
            {
                ComponentManager<T> pool = GetPool<T>();
                T component = pool.GetComponent(entity);
                return component != null;
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
        /// <summary>
        /// Gets the system
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSystem<T>() where T : EcsSystem
        {
            foreach (EcsSystem system in EcsSystems)
                if (typeof(T) == system.GetType()) return (T)system;
            return null;
        }

        /// <summary>
        /// Registers a new system
        /// </summary>
        /// <param name="system"></param>
        public void RegisterSystem(EcsSystem system, int executionOrder)
        {
            int index = 0;
            for(int i = 0; i < EcsSystems.Count; i++)
            {
                if (executionOrder < EcsSystems[i].ExecutionOrder) index = i;
                else break;
            }

            EcsSystems.Insert(index, system);
            system.Setup(this, (uint)index);
        }

        public void Init()
        {
            foreach (EcsSystem system in EcsSystems)
            {
                Core.Profiler.StartTimeMeasure(system.GetType().FullName + ".OnInit()");
                system.OnInit();
                Core.Profiler.StopTimeMeasure(system.GetType().FullName + ".OnInit()");
            }
        }

        public void Tick(float deltaTime)
        {
            foreach (EcsSystem system in EcsSystems)
            {
                Core.Profiler.StartTimeMeasure(system.GetType().FullName + ".OnTick()");
                system.OnTick(deltaTime);
                Core.Profiler.StopTimeMeasure(system.GetType().FullName + ".OnTick()");
            }
        }

        public void EditorTick(float deltaTime)
        {
            foreach (EcsSystem system in EcsSystems)
            {
                Core.Profiler.StartTimeMeasure(system.GetType().FullName + ".OnEditorTick()");
                system.OnEditorTick(deltaTime);
                Core.Profiler.StopTimeMeasure(system.GetType().FullName + ".OnEditorTick()");
            }
        }
        #endregion

        public EcsFilter CreateFilter<T1>() where T1 : IComponent
        {
            Type[] components = new Type[] { typeof(T1) };

            // Check if the filter already Exists
            foreach(EcsFilter filter in cachedFilters)
            {
                Type[] filterComp = filter.Components;
                if (components.Length != filterComp.Length) continue;
                bool areEqual = true;
                for (int i = 0; i < components.Length; i++)
                {
                    if (!filterComp.Contains(components[i]))
                    {
                        areEqual = false;
                        break; ;
                    }
                }

                if (areEqual)
                {
                    return filter;
                }
            }

            EcsFilter newFilter = new EcsFilter(this, components);
            cachedFilters.Add(newFilter);
            return newFilter;
        }

        public EcsFilter CreateFilter<T1, T2>() where T1 : IComponent where T2 : IComponent
        {
            Type[] components = new Type[] { typeof(T1), typeof(T2) };

            // Check if the filter already Exists
            foreach (EcsFilter filter in cachedFilters)
            {
                Type[] filterComp = filter.Components;
                if (components.Length != filterComp.Length) continue;
                bool areEqual = true;
                for (int i = 0; i < components.Length; i++)
                {
                    if (!filterComp.Contains(components[i]))
                    {
                        areEqual = false;
                        break; ;
                    }
                }

                if (areEqual)
                {
                    return filter;
                }
            }

            EcsFilter newFilter = new EcsFilter(this, components);
            cachedFilters.Add(newFilter);
            return newFilter;
        }

        public EcsFilter CreateFilter<T1, T2, T3>() where T1 : IComponent where T2 : IComponent where T3 : IComponent
        {
            Type[] components = new Type[] { typeof(T1), typeof(T2), typeof(T3) };

            // Check if the filter already Exists
            foreach (EcsFilter filter in cachedFilters)
            {
                Type[] filterComp = filter.Components;
                if (components.Length != filterComp.Length) continue;
                bool areEqual = true;
                for (int i = 0; i < components.Length; i++)
                {
                    if (!filterComp.Contains(components[i]))
                    {
                        areEqual = false;
                        break;
                    }
                }

                if (areEqual)
                {
                    return filter;
                }
            }

            EcsFilter newFilter = new EcsFilter(this, components);
            cachedFilters.Add(newFilter);
            return newFilter;
        }

        public EcsFilter CreateFilter(Type[] components) 
        {
            foreach(Type c in components)
            {
                if(c.GetInterface("IComponent") == null)
                {
                    throw new Exception("No Component TSype");
                }
            }

            // Check if the filter already Exists
            foreach (EcsFilter filter in cachedFilters)
            {
                Type[] filterComp = filter.Components;
                if (components.Length != filterComp.Length) continue;
                bool areEqual = true;
                for (int i = 0; i < components.Length; i++)
                {
                    if (!filterComp.Contains(components[i]))
                    {
                        areEqual = false;
                        break;
                    }
                }

                if (areEqual)
                {
                    return filter;
                }
            }

            EcsFilter newFilter = new EcsFilter(this, components);
            cachedFilters.Add(newFilter);
            return newFilter;
        }

        public void Destroy()
        {
            foreach (EcsFilter filter in cachedFilters) filter.IsDead = true;
        }
    }
}
