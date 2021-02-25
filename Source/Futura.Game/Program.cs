using Futura.Engine.Core;
using System;
using System.Threading;

namespace Futura.Game
{
    class Program
    {
        static void Main(string[] args)
        {
            Context context = new Context();
            Time time = context.RegisterSubSystem<Time>();

            time.SetTargetFPS(Time.Mode.Fixed, 60);

            double timing = 0;
            while (true)
            {
                context.Tick(Context.TickType.Variable, time.DeltaTime);

                ///Thread.Sleep(1);

                timing += time.DeltaTime;
                if(timing > 5000)
                {
                    timing = 0;
                    Console.WriteLine("Variable: " + 1000 / time.DeltaTime + " Smoothed " + 1000 / time.DeltaTimeSmoothed + " - " + time.DeltaTimeSmoothed);
                }

            }

        }
    }
}
