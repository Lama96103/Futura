using System;
using System.Collections.Generic;
using System.Text;

namespace Futura
{
    /// <summary>
    /// If you apply this attribute to any field, propery, class or struct it will not get shown in the editor
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class IgnoreAttribute : System.Attribute
    {
    }

    /// <summary>
    /// IF you apply this attribute to a private variable it will still get serialized in the editor
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SerializeField : System.Attribute
    {

    }

    /// <summary>
    /// Attribute will set the name of the field, property, class in the editor
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class NameAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public NameAttribute(string name)
        {
            this.Name = name;
        }
    }

    /// <summary>
    /// Attribute will enable a slider for the given value
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class RangeAttribute : System.Attribute
    {
        public float Min { get; private set; } = 0;
        public float Max { get; private set; } = 1;
        public float Step { get; set; } = 0.1f;


        public RangeAttribute(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }

        public RangeAttribute(float min, float max, float step)
        {
            this.Min = min;
            this.Max = max;
            this.Step = step;
        }
    }

  
}
