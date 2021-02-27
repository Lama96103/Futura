using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.UserInterface
{
    public sealed class UIController : Singleton<UIController>
    {
        private int IdCounter = 0;

        private List<View> views = new List<View>();
        private List<View> toRemove = new List<View>();
        private List<View> toAdd = new List<View>();

   
        internal void Tick()
        {
            foreach (View view in toRemove)
                views.Remove(view);
            toRemove.Clear();

            foreach (View view in toAdd)
            {
                views.Add(view);
                view.Init();
            }
            toAdd.Clear();

            foreach (View view in views)
                view.Tick();
        }

  

        public void Register(View view)
        {
            toAdd.Add(view);
            IdCounter++;
            view.ID = IdCounter.ToString();
        }

        public void Unregister(View view)
        {
            if (views.Contains(view)) toRemove.Add(view);
        }

        internal void Unload()
        {
            foreach (View view in views)
                Unregister(view);
        }
    }

    public abstract class View
    {
        public string ID { get; set; } = "empty";

        public virtual void Init() { }

        public abstract void Tick();
    }
}
