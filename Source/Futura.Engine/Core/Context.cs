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
            T system = Activator.CreateInstance<T>();
            system.Register(this, tick);

            subSystems.Add(system);

            return system;
        }

        public void Tick(TickType tickType, double deltaTime)
        {
            foreach(SubSystem sub in subSystems)
            {
                if (sub.TickType != tickType) continue;
                sub.Tick(deltaTime);
            }
        }


        public enum TickType
        {
            Variable, Smoothed
        }
    }
}
