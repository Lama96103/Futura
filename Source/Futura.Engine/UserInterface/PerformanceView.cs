using Futura.Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.UserInterface
{
    class PerformanceView : View
    {
        private string txt_WindowName;

        public override void Init()
        {
            txt_WindowName = "Performance##" + ID;
        }

        public override void Tick()
        {
            ImGui.Begin(txt_WindowName, ref isOpen);

            var measuredTimes = Profiler.MeasuredTime;

            ImGui.Columns(2);
            foreach(var time in measuredTimes)
            {
                ImGui.Text(time.Key);
                ImGui.NextColumn();
                ImGui.Text(time.Value.ElapsedMilliseconds.ToString());
                ImGui.NextColumn();
            }

            ImGui.End();
        }
    }
}
