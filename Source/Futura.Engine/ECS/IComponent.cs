using System;
using System.Collections.Generic;
using System.Text;

namespace Futura.Engine.ECS
{
    public interface IComponent
    {
    }

    public interface ICustomUserInterface
    {
        public bool Display();
    }

    public interface IComponentChangeListener
    {
        public void OnChanged();
    }
}
