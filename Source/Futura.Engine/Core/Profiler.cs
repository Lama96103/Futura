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
        [Conditional("PROFILE")]
        internal static void StartFrame()
        {
        }

        [Conditional("PROFILE")]
        internal static void EndFrame()
        {

        }

        [Conditional("PROFILE")]
        public static void Report(string eventName, string eventParams = "")
        {
            Log.Debug($"Event: {eventName}>{eventParams}");
        }
    }
}
