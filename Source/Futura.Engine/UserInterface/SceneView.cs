using Futura.ECS;
using Futura.Engine.Core;
using Futura.Engine.Rendering;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Futura.Engine.UserInterface
{
    public class SceneView : View
    {
        private IntPtr colorImagePointer = IntPtr.Zero;

        private Vector2 imageSize = Vector2.Zero;

        private RenderSystem renderSystem;
        private TimeSystem timeSystem;

        public override void Init()
        {
            renderSystem = Runtime.Instance.Context.GetSubSystem<RenderSystem>();
            timeSystem = Runtime.Instance.Context.GetSubSystem<TimeSystem>();
            colorImagePointer = ImGuiController.Instance.GetOrCreateImGuiBinding(renderSystem.DiffuseFrameBuffer.ColorTexture.Handle);
        }

        public override void Tick()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0));
            ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.MenuBar;


            ImGui.Begin("Scene##" + ID, flags);
            ImGui.PopStyleVar();

            ImGui.BeginChild("GameRender");

            if (imageSize != ImGui.GetWindowSize())
            {
                imageSize = ImGui.GetWindowSize();
                renderSystem.ResizeSceneRendering((uint)imageSize.X, (uint)imageSize.Y);

                Log.Debug("Scene Size: " + imageSize.X + " " + imageSize.Y);
            }

            ImGui.Image(colorImagePointer, imageSize);

            ImGui.EndChild();
            ShowRenderPerformance();
            ImGui.End();
        }

        private void ShowRenderPerformance()
        {
            ImGui.SetCursorPos(ImGui.GetCursorStartPos() + new Vector2(5));

            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(1, 1, 1, 0.5f));
            ImGui.BeginChild("performanceoverlay", new Vector2(260, 130), false, ImGuiWindowFlags.NoDecoration);


            ImGui.LabelText("API", renderSystem.API.CurrentAPI);
            ImGui.LabelText("FPS", timeSystem.FPS.ToString("0.00"));
            ImGui.LabelText("Delta", timeSystem.DeltaTime.ToString("0.00"));
            ImGui.LabelText("Res", renderSystem.DiffuseFrameBuffer.Width + "x" + renderSystem.DiffuseFrameBuffer.Height);
            ImGui.LabelText("Draw Calls", Profiler.GetIndicator(Profiler.StatisticIndicator.DrawCall).ToString());
            ImGui.LabelText("Vertices", Profiler.GetIndicator(Profiler.StatisticIndicator.Vertex).ToString());


            ImGui.EndChild();

            ImGui.PopStyleColor();
        }

    }
}
