using Futura.Engine.ECS.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Futura.Engine.ECS
{
    internal interface IComponentManager 
    {
        internal Type GetComponentType();

        internal void RemoveEntity(Entity entity);
        internal IComponent GetComponent(Entity entity);
    }


    internal class ComponentManager<T> : IComponentManager where T : class, IComponent
    {
        [JsonProperty] private Dictionary<int, T> componentData = new Dictionary<int, T>();
        [JsonProperty] private Type Type = typeof(T);

        public ComponentManager(){}

        public event EventHandler<ComponentEventArgs<T>> ComponentCreated;
        public event EventHandler<ComponentEventArgs<T>> ComponentRemoved;

        /// <summary>
        /// Tries to get the component for the entity. If the entity does not have that component it will return null
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal T GetComponent(Entity entity)
        {
            if (!entity.IsValid) throw new EntityNotValidException();

            if (componentData.TryGetValue(entity.ID, out T value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// First tries to get the component if it not exist it will get created
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal T Get(Entity entity)
        {
            if (!entity.IsValid) throw new EntityNotValidException();

            if (componentData.ContainsKey(entity.ID)) return GetComponent(entity);

            T component = (T)Activator.CreateInstance(typeof(T));
            componentData.Add(entity.ID, component);
            ComponentCreated?.Invoke(this, new ComponentEventArgs<T>(entity, component));

            return component;
        }

        internal void Remove(Entity entity)
        {
            if (!entity.IsValid) throw new EntityNotValidException();

            if(componentData.TryGetValue(entity.ID, out T value))
            {
                ComponentRemoved?.Invoke(this, new ComponentEventArgs<T>(entity, value));
                componentData.Remove(entity.ID);
            }
        }

        Type IComponentManager.GetComponentType()
        {
            return Type;
        }

        void IComponentManager.RemoveEntity(Entity entity)
        {
            Remove(entity);
        }

        IComponent IComponentManager.GetComponent(Entity entity)
        {
            return GetComponent(entity);
        }
    }

    internal class ComponentEventArgs<T> : EventArgs where T : class, IComponent
    {
        public ComponentEventArgs(Entity entity, T component)
        {
            Entity = entity;
            Component = component;
        }

        public Entity Entity { get; private set; }
        public T Component { get; private set; }


    }

}
