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
        public async static void CreateAsyncTask(Action action, string name)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            await Task.Run(action);
            stopwatch.Stop();
            // TODO Capture this data in the profiler
        }

        public static Task CreateTask(Action action, string name)
        {
            Task task = Task.Run(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                action.Invoke();
                stopwatch.Stop();
            });
            return task;
            // TODO Capture this data in the profiler
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
