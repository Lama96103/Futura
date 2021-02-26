using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Core
{
    public sealed class Runtime : Singleton<Runtime>
    {
        public Context Context { get; init; }

        private Time time;

        public Runtime()
        {
            Context = new Context();
        }


        public void Init()
        {
            time = Context.RegisterSubSystem<Time>();
            Context.RegisterSubSystem<Logger>();
        }

        public void Tick()
        {
            Context.Tick(Context.TickType.Variable, time.DeltaTime);
            Context.Tick(Context.TickType.Smoothed, time.DeltaTimeSmoothed);
        }
    }
}
