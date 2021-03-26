using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Core
{
    public sealed class Context
    {
        private List<SubSystem> subSystems = new List<SubSystem>();



        public T RegisterSubSystem<T>(TickType tick = TickType.Variable) where T : SubSystem
        {
            if (GetSubSystem<T>() != null) return GetSubSystem<T>();

            T system = Activator.CreateInstance<T>();
            system.Register(this, tick);

            subSystems.Add(system);

            return system;
        }

        public T GetSubSystem<T>() where T : SubSystem
        {
            foreach(SubSystem s in subSystems)
            {
                if (s.GetType() == typeof(T)) return (T)s;
            }
            return null;
        }

        internal void Init()
        {
            foreach (SubSystem sub in subSystems)
            {
                sub.Init();
            }
        }

        internal void Tick(TickType tickType, double deltaTime)
        {
            if (deltaTime <= 0) deltaTime = 0.0001f;
            foreach(SubSystem sub in subSystems)
            {
                if (sub.TickType != tickType) continue;

                try
                {
                    string identifier = sub.GetType().FullName + ".Tick()";
                    Profiler.StartTimeMeasure(identifier);
                    sub.Tick(deltaTime);
                    Profiler.StopTimeMeasure(identifier);
                }
                catch(Exception e)
                {
                    Log.Error("Could not exceute " + sub.GetType().Name, e);
                }

            }
        }


        public enum TickType
        {
            Variable, Smoothed
        }
    }
}
