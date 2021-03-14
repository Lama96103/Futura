using Futura.Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.UserInterface
{
    class PerformanceView : View
    {
        private string txt_WindowName;

        private string filter = string.Empty;

        public override void Init()
        {
            txt_WindowName = "Performance##" + ID;
        }

        public override void Tick()
        {
            ImGui.Begin(txt_WindowName, ref isOpen);

            if (ImGui.InputText("Filter", ref filter, 300)) { }


            var measuredTimes = Profiler.MeasuredTime;

            Vector2 windowSize = ImGui.GetWindowSize();
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, windowSize.X - 30);
            ImGui.SetColumnWidth(1, 50);

            ImGui.TextDisabled("Function");
            ImGui.NextColumn();
            ImGui.TextDisabled("ms");
            ImGui.NextColumn();

            foreach (var time in measuredTimes)
            {
                if (!time.Key.Contains(filter)) continue;
                ImGui.Text(time.Key);
                ImGui.NextColumn();
                ImGui.Text(time.Value.ElapsedMilliseconds.ToString());
                ImGui.NextColumn();
            }

            ImGui.End();
        }
    }
}
