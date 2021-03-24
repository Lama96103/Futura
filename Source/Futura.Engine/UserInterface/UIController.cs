﻿using Futura.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            Profiler.StartTimeMeasure(typeof(UIController).FullName + ".Tick()");
            foreach (View view in toRemove)
            {
                view.OnDestroy();
                views.Remove(view);
            }
            toRemove.Clear();

            foreach (View view in toAdd)
            {
                views.Add(view);
                view.Init();
            }
            toAdd.Clear();

            foreach (View view in views)
            {
                if (view.IsOpen)
                {
                    Profiler.StartTimeMeasure(view.GetType().FullName + ".Tick()");
                    view.Tick();
                    Profiler.StopTimeMeasure(view.GetType().FullName + ".Tick()");
                }
                else
                    toRemove.Add(view);
            }
            Profiler.StopTimeMeasure(typeof(UIController).FullName + ".Tick()");
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

        protected bool isOpen = true;
        public bool IsOpen { get => isOpen; set => isOpen = value; }

        public virtual void Init() { }

        public abstract void Tick();
        public virtual void OnDestroy() { }

        protected void SetInitalWindowSize(float width, float height)
        {
            ImGuiNET.ImGui.SetNextWindowSize(new Vector2(width, height), ImGuiNET.ImGuiCond.Once);
        }
    }
}
