using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Futura.ECS
{
    public class EcsFilter
    {
        private EcsWorld world;
        private Type[] components;

        private List<EntityReference> entityReferences = new List<EntityReference>();

        public IEnumerable<EntityReference> Entities
        {
            get => entityReferences.Where(e => e.IsComplete);
        }

        public EcsFilter(EcsWorld world, IComponent[] components)
        {
            this.world = world;

            List<Type> types = new List<Type>();

            foreach(var com in components)
            {
                if (!types.Contains(com.GetType())) types.Add(com.GetType());
            }
            this.components = types.ToArray();


            foreach(Type t in this.components)
            {
                var attachEventHandlerMethod = typeof(EcsFilter).GetMethod("AttachEventHandler");
                var attachEventHandler = attachEventHandlerMethod.MakeGenericMethod(t);
                attachEventHandler.Invoke(this, null);
            }
        }

        public void AttachEventHandler<T>() where T : class, IComponent
        {
            ComponentManager<T> pool = world.GetPool<T>();
            pool.ComponentCreated += (object sender, ComponentEventArgs<T> e) => 
            {
                Type type = typeof(T);

                var entites = entityReferences.Where(entity => entity.Entity == e.Entity).ToArray();

                // Get the entity refernce
                EntityReference reference = null;
                if (entites.Length == 0)
                {
                    reference = new EntityReference()
                    {
                        Entity = e.Entity,
                        Components = new IComponent[components.Length]
                    };
                    entityReferences.Add(reference);
                }
                else if(entites.Length == 1)
                {
                    reference = entites[0];
                }
                else
                {
                    throw new Exception("Multiple Entity references were found");
                }

                // Get the component index
                int index = GetComponentIndex(type);

                // Add the component
                reference.Components[index] = e.Component;

                bool isComplete = true;
                foreach(var comp in reference.Components)
                {
                    if (comp == null) isComplete = false;
                }

                reference.IsComplete = isComplete;
            };
            pool.ComponentRemoved += (object sender, ComponentEventArgs<T> e) =>
            {
                Type type = typeof(T);

                var entites = entityReferences.Where(entity => entity.Entity == e.Entity).ToArray();

                // Get the entity refernce
                EntityReference reference = null;
                if (entites.Length == 0)
                {
                    return;
                }
                else if (entites.Length == 1)
                {
                    reference = entites[0];
                }
                else
                {
                    throw new Exception("Multiple Entity references were found");
                }

                // Get the component index
                int index = GetComponentIndex(type);

                // Remove the component
                reference.Components[index] = null;

                bool isComplete = true;
                bool isEmpty = true;
                foreach (var comp in reference.Components)
                {
                    if (comp == null) isComplete = false;
                    if (comp != null) isEmpty = false;
                }

                reference.IsComplete = isComplete;
                
                if (isEmpty)
                    entityReferences.Remove(reference);
                
            };

            // Checks for already existing components
            foreach(var entity in world.Entities)
            {
                var comp = pool.GetComponent(entity);

                var references = entityReferences.Where(e => e.Entity == entity).ToArray();

                // Get the entity refernce
                EntityReference reference = null;
                if (references.Length == 0)
                {
                    reference = new EntityReference()
                    {
                        Entity = entity,
                        Components = new IComponent[components.Length]
                    };
                    entityReferences.Add(reference);
                }
                else if (references.Length == 1)
                {
                    reference = references[0];
                }
                else
                {
                    throw new Exception("Multiple Entity references were found");
                }

                // Get the component index
                int index = GetComponentIndex(typeof(T));

                // Add the component
                reference.Components[index] = comp;

                bool isComplete = true;
                foreach (var c in reference.Components)
                {
                    if (c == null) isComplete = false;
                }

                reference.IsComplete = isComplete;
            }
        }

        private int GetComponentIndex(Type type)
        {
            int index = -1;
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == type)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1) throw new Exception("Type not present in filter");
            return index;
        }

        public class EntityReference
        {
            public Entity Entity = null;
            public IComponent[] Components;
            public bool IsComplete = false;

            public T GetComponent<T>() where T : class, IComponent
            {
                foreach(var c in Components)
                {
                    if(typeof(T) == c.GetType()) return (T)c;
                }
                return null;
            }
        }

    }
}
