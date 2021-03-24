using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Utility
{
    class CycleArray<T>
    {
        public T[] Array;

        public int Length { get; private set; } = 0;
        public int CurrentIndex { get; private set; } = -1;
        public bool DidReachFinish { get; private set; } = false;

        public CycleArray(int length)
        {
            Array = new T[length];
        }

        public void Push(T value)
        {
            CurrentIndex++;
            if(CurrentIndex >= Array.Length)
            {
                CurrentIndex = 0;
                DidReachFinish = true;
            }
            Length = DidReachFinish ? Array.Length : CurrentIndex + 1;

            Array[CurrentIndex] = value;
        }
    }

    class ImGuiPlotArray
    {
        public float[] Array;

        public int Length { get; private set; } = 0;
        public int CurrentIndex { get; private set; } = -1;
        public bool DidReachFinish { get; private set; } = false;

        public float MaxValue = 0;
        public float MinValue = float.MaxValue;

        public ImGuiPlotArray(int length)
        {
            Array = new float[length];
        }

        public void Push(float value)
        {
            CurrentIndex++;
            if (CurrentIndex >= Array.Length)
            {
                CurrentIndex = Array.Length - 1;
                DidReachFinish = true;
            }

            if (DidReachFinish)
            {
                float[] newArray = new float[Array.Length];

                System.Array.Copy(Array, 1, newArray, 0, Array.Length - 1);
                Array = newArray;
                Array[Array.Length-1] = value;
            }
            else
            {
                Length = DidReachFinish ? Array.Length : CurrentIndex + 1;
                Array[CurrentIndex] = value;
            }

            if (value > MaxValue) MaxValue = value;
            else if (value < MinValue) MinValue = value;
        }
    }
}
