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

        private TimeSystem timeSys;

        public Runtime()
        {
            Context = new Context();
        }


        public void Init()
        {
            timeSys = Context.RegisterSubSystem<TimeSystem>();
            Context.RegisterSubSystem<LogSystem>();
            Context.RegisterSubSystem<WorldSystem>();
            Context.RegisterSubSystem<RenderSystem>();



            Context.Init();
        }

        public void Tick()
        {
            Context.Tick(Context.TickType.Variable, timeSys.DeltaTime);
            Context.Tick(Context.TickType.Smoothed, timeSys.DeltaTimeSmoothed);
        }
    }
}
