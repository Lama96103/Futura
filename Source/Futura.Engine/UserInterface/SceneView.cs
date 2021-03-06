﻿using Futura.ECS;
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
        private string txt_WindowName;
        private string txt_SceneChild;
        private string txt_OverlayChild;

        private IntPtr colorImagePointer = IntPtr.Zero;

        private Vector2 imageSize = Vector2.Zero;


        private RenderSystem renderSystem;
        private TimeSystem timeSystem;

        private bool startedOnItem = false;

        public override void Init()
        {
            renderSystem = Runtime.Instance.Context.GetSubSystem<RenderSystem>();
            timeSystem = Runtime.Instance.Context.GetSubSystem<TimeSystem>();
            colorImagePointer = ImGuiController.Instance.GetOrCreateImGuiBinding(renderSystem.DiffuseFrameBuffer.ColorTexture.Handle);

            txt_WindowName = $"Scene##{ID}";
            txt_SceneChild = $"GameRender##{ID}";
            txt_OverlayChild = $"PerformanceOverlay##{ID}";

        }

        public override void Tick()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0));
            ImGuiWindowFlags flags = ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.MenuBar;


            ImGui.Begin(txt_WindowName, flags);
            ImGui.PopStyleVar();

            ImGui.BeginChild(txt_SceneChild);

            if (imageSize != ImGui.GetWindowSize())
            {
                imageSize = ImGui.GetWindowSize();
                renderSystem.ResizeRenderResolution((uint)imageSize.X, (uint)imageSize.Y);
                Log.Debug("Scene Size: " + imageSize.X + " " + imageSize.Y);
            }

            if (ImGuiController.Instance.ClearedCache)
            {
                colorImagePointer = ImGuiController.Instance.GetOrCreateImGuiBinding(renderSystem.DiffuseFrameBuffer.ColorTexture.Handle);
            }

            ImGui.Image(colorImagePointer, imageSize);

            if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                if (ImGui.IsItemHovered()) startedOnItem = true;

                if(startedOnItem)
                    EditorCamera.Instance.Tick();
            }
            else
            {
                startedOnItem = false;
            }

            if (Input.IsKeyDown(Veldrid.Key.F1)) renderSystem.UseEditorCamera = !renderSystem.UseEditorCamera;

            ImGui.EndChild();

            ShowRenderPerformance();
            ImGui.End();
        }

        private void ShowRenderPerformance()
        {
            ImGui.SetCursorPos(ImGui.GetCursorStartPos() + new Vector2(5));

            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.5f, 0.5f, 1, 0.5f));
            ImGui.BeginChild(txt_OverlayChild, new Vector2(200, 100), false, ImGuiWindowFlags.NoDecoration);

            ImGui.Columns(2);
            ImGui.Text("API");
            ImGui.Text("FPS");
            ImGui.Text("Delta");
            ImGui.Text("Res");
            ImGui.Text("Draw Calls");
            ImGui.Text("Vertices");

            ImGui.NextColumn();

            ImGui.Text(renderSystem.API.CurrentAPI);
            ImGui.Text(timeSystem.FPS.ToString("0.00"));
            ImGui.Text(timeSystem.DeltaTime.ToString("0.00"));
            ImGui.Text(renderSystem.DiffuseFrameBuffer.Width + "x" + renderSystem.DiffuseFrameBuffer.Height);
            ImGui.Text(Profiler.GetIndicator(Profiler.StatisticIndicator.DrawCall).ToString());
            ImGui.Text(Profiler.GetIndicator(Profiler.StatisticIndicator.Vertex).ToString());


            ImGui.EndChild();

            ImGui.PopStyleColor();
        }

    }
}
