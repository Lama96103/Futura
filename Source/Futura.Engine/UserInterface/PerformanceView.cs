using Futura.Engine.Core;
using Futura.Engine.Utility;
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
        

        private ImGuiPlotArray memoryBuffer = new ImGuiPlotArray(1000);
        private ImGuiPlotArray frameTimes = new ImGuiPlotArray(300);

        private string filter = string.Empty;

        public override void Init()
        {
            txt_WindowName = "Performance##" + ID;
        }

        public override void Tick()
        {
            Begin(txt_WindowName, ref isOpen);

            Vector2 windowSize = ImGui.GetWindowSize();

            memoryBuffer.Push((float)(Profiler.WorkingSet / 1000 / 1000));
            frameTimes.Push(Time.DeltaTime);

            ImGui.PlotLines("##DeltaTime", ref frameTimes.Array[0], frameTimes.Length, 0, "Deltatime (ms)", 0, 45f, new Vector2(windowSize.X / 2, 75));
            ImGui.SameLine();
            ImGui.PlotLines("##Memory", ref memoryBuffer.Array[0], memoryBuffer.Length, 0, "Memory (MB)", memoryBuffer.MinValue, memoryBuffer.MaxValue, new Vector2(windowSize.X/2, 75));

            ImGui.Separator();

            if (ImGui.InputText("Filter", ref filter, 400)) { }

            if (ImGui.BeginChild("PerformanceFilter"))
            {


                var measuredTimes = Profiler.MeasuredTime;

                ImGui.Columns(2);
                ImGui.SetColumnWidth(0, windowSize.X - 100);
                ImGui.SetColumnWidth(1, 100);

                ImGui.TextDisabled("Function");
                ImGui.NextColumn();
                ImGui.TextDisabled("ms");
                ImGui.NextColumn();

                foreach (var time in measuredTimes)
                {
                    if (!time.Key.Contains(filter)) continue;
                    ImGui.Text(time.Key);
                    ImGui.NextColumn();
                    ImGui.Text(time.Value.Elapsed.TotalMilliseconds.ToString("0.0000"));
                    ImGui.NextColumn();
                }

            }           
            ImGui.EndChild();

            End();
        }
    }
}
