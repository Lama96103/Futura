using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Futura.Engine.Core;

namespace Futura.Engine.Core
{
    public sealed class TimeSystem : SubSystem
    {
        private Stopwatch totalTime;
        private Stopwatch frameTime;

        public double MinFPS { get; private set; } = 30.0;
        public double MaxFPS { get; private set; } = 1000.0;
        public double TargetFPS { get; private set; } = 1000.0;

        public Mode ModeFPS { get; private set; } = Mode.Unlocked;

        public double DeltaTime { get => deltaTime; }
        public double DeltaTimeSmoothed { get => deltaTimeSmoothed; }
        public double FPS { get => 1000.0 / DeltaTime; }
        public TimeSpan TotalTime { get => totalTime.Elapsed; }

        // The delta times are in ms
        private double deltaTime = 0.0f;
        private double deltaTimeSmoothed = 0.0f;

        double sleepOverhead = 0.0f;

        public TimeSystem()
        {
            totalTime = Stopwatch.StartNew();
            frameTime = Stopwatch.StartNew();
            Time.Init(this);
        }


        internal override void Tick(double deltaTime)
        {
            TimeSpan time = frameTime.Elapsed;
            frameTime.Restart();

            double remainingTime = (1000.0 / TargetFPS) - time.TotalMilliseconds;

            if(remainingTime > 0.0)
            {
                Stopwatch overhead = Stopwatch.StartNew();
                
                double requested = remainingTime - sleepOverhead;
                if(requested > 0)
                    Thread.Sleep((int)requested);

                sleepOverhead = overhead.ElapsedMilliseconds;
            }

            this.deltaTime = time.TotalMilliseconds;

            // Compute smoothed delta time
            double framesToAccumulate = 5;
            double deltaFeedback = 1.0 / framesToAccumulate;
            double deltaMax = 1000.0 / MinFPS;
            double deltaClamped = deltaTime > deltaMax ? deltaMax : deltaTime;
            this.deltaTimeSmoothed = deltaTimeSmoothed * (1.0 - deltaFeedback) + deltaClamped * deltaFeedback;
        }

        public void SetTargetFPS(Mode mode, double fps = 0)
        {
            if(mode == Mode.FixedMonitor)
            {
                ModeFPS = Mode.FixedMonitor;
                // TODO Get Display Refresh Rate;
                fps = 60;
            }
            else if(mode == Mode.Unlocked)
            {
                ModeFPS = Mode.Unlocked;
                fps = MaxFPS;
            }
            else
            {
                ModeFPS = Mode.Fixed;
            }

            TargetFPS = fps;
        }


        public enum Mode
        {
            Unlocked, Fixed, FixedMonitor
        }
    }
}

namespace Futura
{
    public static class Time
    {
        private static TimeSystem timeSys;

        internal static void Init(TimeSystem timeSys)
        {
            Time.timeSys = timeSys;
        }

        /// <summary>
        /// DeltaTime in ms
        /// </summary>
        public static float DeltaTime { get => (float)timeSys.DeltaTime; }

        /// <summary>
        /// DeltaTime in ms
        /// </summary>
        public static double DeltaTime_d { get => timeSys.DeltaTime; }

        /// <summary>
        /// DeltaTime in s
        /// </summary>
        public static float DeltaSeconds { get => (float)timeSys.DeltaTime/1000.0f; }
    }
}
