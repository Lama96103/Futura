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
        private static Dictionary<string, Stopwatch> measuredTimes = new Dictionary<string, Stopwatch>();

        private static Process currentProcess = Process.GetCurrentProcess();

        public static long WorkingSet { get; private set; } = 0;
        public static TimeSpan TotalProcessorTime { get; private set; } = TimeSpan.Zero;
        public static long PagedMemorySize { get; private set; } = 0;
        public static long PagedSystemMemorySize { get; private set; } = 0;
        public static long PrivateMemorySize { get; private set; } = 0;

        [Conditional("PROFILE")]
        internal static void StartFrame()
        {
            indicatorValues.Clear();
            for(int i = 0; i <= (int)Enum.GetValues<StatisticIndicator>().Max(); i++)
            {
                indicatorValues.Add((StatisticIndicator)i, 0);
            }

            currentProcess.Refresh();
            WorkingSet = currentProcess.WorkingSet64;
            TotalProcessorTime = currentProcess.TotalProcessorTime;
            PagedMemorySize = currentProcess.PagedMemorySize64;
            PagedSystemMemorySize = currentProcess.PagedSystemMemorySize64;
            PrivateMemorySize = currentProcess.PrivateMemorySize64;

            measuredTimes.Clear();
        }

        [Conditional("PROFILE")]
        internal static void EndFrame()
        {
            lastFrame = indicatorValues;
        }

        internal static int GetIndicator(StatisticIndicator indicator)
        {
            if (lastFrame.ContainsKey(indicator))
                return lastFrame[indicator];
            else return 0;
        }

        internal static Dictionary<string, Stopwatch> MeasuredTime { get => measuredTimes; }

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

        [Conditional("PROFILE")]
        public static void StartTimeMeasure(string key)
        {
            if (measuredTimes.ContainsKey(key))
            {
                measuredTimes[key].Restart();
            }
            else
            {
                measuredTimes.Add(key, Stopwatch.StartNew());
            }
        }

        [Conditional("PROFILE")]
        public static void StopTimeMeasure(string key)
        {
            if (measuredTimes.ContainsKey(key))
            {
                measuredTimes[key].Stop();
            }
        }

        public enum StatisticIndicator
        {
            DrawCall,
            Vertex,
            BuildMesh,
            Load_Mesh,
            Load_Texture
        }
    }
}
