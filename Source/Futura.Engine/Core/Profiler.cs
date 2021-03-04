using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Core
{
    public static class Profiler
    {
        private static readonly Dictionary<StatisticIndicator, int> indicatorValues = new Dictionary<StatisticIndicator, int>();
        private static Dictionary<StatisticIndicator, int> lastFrame = new Dictionary<StatisticIndicator, int>();

        [Conditional("PROFILE")]
        internal static void StartFrame()
        {
            indicatorValues.Clear();
            for(int i = 0; i <= (int)Enum.GetValues<StatisticIndicator>().Max(); i++)
            {
                indicatorValues.Add((StatisticIndicator)i, 0);
            }
        }

        [Conditional("PROFILE")]
        internal static void EndFrame()
        {
            lastFrame = indicatorValues;
        }

        [Conditional("PROFILE")]
        public static void Report(string eventName, string eventParams = "")
        {
            Log.Debug($"Event: {eventName}>{eventParams}");
        }

        [Conditional("PROFILE")]
        public static void Report(StatisticIndicator indicator, int count = 1)
        {
            indicatorValues[indicator] += count;
        }

        public static int GetIndicator(StatisticIndicator indicator)
        {
            if (lastFrame.ContainsKey(indicator))
                return lastFrame[indicator];
            else return 0;
        }


        public enum StatisticIndicator
        {
            DrawCall,
            Vertex
        }
    }
}
