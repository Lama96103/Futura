using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Futura.Engine.Core
{
    public static class ThreadController
    {
        public async static void CreateTask(Action action, string name)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            await Task.Run(action);
            stopwatch.Stop();

            Log.Trace("Task " + name + " took " + stopwatch.ElapsedMilliseconds + " ms");
        }

        public static void Sleep(int ms)
        {
            Thread.Sleep(ms);
        }

        public static void Sleep(TimeSpan time)
        {
            Thread.Sleep(time);
        }
    }
}
