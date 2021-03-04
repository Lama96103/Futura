using Futura.Engine.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Futura.Engine.Core.LogSystem;

namespace Futura.Engine.UserInterface
{
    public sealed class LogView : View
    {
        private string txt_WindowName;
        private string txt_LogChild;
        private string txt_ClearButton;
        

        private static readonly Vector4 DebugColor = new Vector4(0.8f);
        private static readonly Vector4 InfoColor = new Vector4(1);
        private static readonly Vector4 WarningColor = new Vector4(1, 1, 0, 1);
        private static readonly Vector4 ErrorColor = new Vector4(1, 0, 0, 1);

        private bool ShowError = true;
        private bool ShowWarning = true;
        private bool ShowInfo = true;
        private bool ShowDebug = true;

        private List<LogMessage> messages = new List<LogMessage>();

        public LogView()
        {
            Core.LogSystem.OnLogReceived += Logger_OnLogReceived;
        }

        public override void Init()
        {
            txt_WindowName = $"Log View##{ID}";
            txt_LogChild = $"LogView_Messages##{ID}";
            txt_ClearButton = $"Clear##{ID}";
        }

        public override void Tick()
        {
            SetInitalWindowSize(200, 100);
            ImGui.Begin(txt_WindowName, ref isOpen);

            if (ImGui.Checkbox("Error", ref ShowError)) { }
            ImGui.SameLine();
            if (ImGui.Checkbox("Warning", ref ShowWarning)) { }
            ImGui.SameLine();
            if (ImGui.Checkbox("Info", ref ShowInfo)) { }
            ImGui.SameLine();
            if (ImGui.Checkbox("Debug", ref ShowDebug)) { }
            ImGui.SameLine();
            if (ImGui.Button(txt_ClearButton))
            {
                messages.Clear();
            }

            ImGui.BeginChild(txt_LogChild, new Vector2(0.1f, 0.1f), true);

            foreach (var m in messages)
            {
                Vector4 color = InfoColor;
                switch (m.Level)
                {
                    case LogLevel.Debug:
                        color = DebugColor;
                        if (!ShowDebug) continue;
                        break;
                    case LogLevel.Warning:
                        color = WarningColor;
                        if (!ShowWarning) continue;
                        break;
                    case LogLevel.Info:
                        if (!ShowInfo) continue;
                        break;
                    case LogLevel.Error:
                        color = ErrorColor;
                        if (!ShowError) continue;
                        break;
                    default:
                        break;
                }

                ImGui.TextColored(color, $"{m.Timestamp.ToString("HH:mm:ss.fff")} {m.Message}");
            }

            ImGui.SetScrollHereY();

            ImGui.EndChild();

            ImGui.End();
        }

        public override void OnDestroy()
        {
            Core.LogSystem.OnLogReceived -= Logger_OnLogReceived;
        }

        private void Logger_OnLogReceived(object sender, LogEventArgs e)
        {
            messages.Add(e.LogMsg);
        }
    }
}
