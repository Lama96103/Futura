using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Core
{
    public abstract class SubSystem
    {
        public Context Context { get; private set; }
        public Context.TickType TickType { get; private set; }

        internal void Register(Context context, Context.TickType type)
        {
            this.Context = context;
            this.TickType = type;
        }

       
        internal virtual void Init() { }
        internal abstract void Tick(double deltaTime);
    }
}
