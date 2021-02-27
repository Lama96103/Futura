using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Futura.ECS
{
    public class Entity
    {
        [JsonProperty] public int ID { get; private set; }
        [JsonIgnore] private EcsWorld world;

        #region Constructors
        internal Entity() { }
        internal Entity(EcsWorld world, int id)
        {
            this.world = world;
            this.ID = id;
        }
        #endregion

        /// <summary>
        /// Checks if the entity is valid
        /// </summary>
        [JsonIgnore] public bool IsValid
        {
            get => world != null && ID != 0;
        }

        /// <summary>
        /// Gets the component. If it doesn't exist it will get created
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : class, IComponent
        {
            return world.GetComponent<T>(this);
        }

        public void AddComponent(Type t)
        {
            if(t.IsClass && t.GetInterface("Futura.ECS.IComponent") != null)
            {
                var method = typeof(EcsWorld).GetMethod("GetComponent", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(t);
                method.Invoke(world, new object[] { this });
            }
        }

        /// <summary>
        /// Removes component from entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveComponent<T>() where T : class, IComponent
        {
            world.RemoveComponent<T>(this);
        }

        /// <summary>
        /// Get all components related to the entity
        /// </summary>
        /// <returns></returns>
        public List<IComponent> GetAllComponents()
        {
            return world.GetComponents(this);
        }

        /// <summary>
        /// Returns the name of the entiy
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Entity ({ID})";
        }

        /// <summary>
        /// Sets the entity to world. Only need to get called after scene is loaded from file
        /// </summary>
        /// <param name="world"></param>
        internal void SetWorld(EcsWorld world)
        {
            this.world = world;
        }
    }
}
