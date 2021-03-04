using Futura.ECS;
using Futura.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Core
{
    partial class RenderSystem : SubSystem
    {
        private RenderAPI renderAPI;
        internal RenderAPI API { get => renderAPI;}

        private DeviceResourceCache deviceResourceCache;

        private int resolutionWidth;
        private int resolutionHeight;

        private EcsFilter entityFilter;
        private EcsFilter cameraFilter;

        private CommandList diffuseCommandList;
        private CommandList uiCommandList;

        bool isWindowResized = false;
        uint newWindowWidth = 0;
        uint newWindowHeight = 0;

        internal override void Init()
        {
            renderAPI = new RenderAPI(this.Context, Window.Instance);
            resolutionWidth = Window.Instance.Width;
            resolutionHeight = Window.Instance.Height;
            Window.Instance.WindowResized += WindowResized;

            deviceResourceCache = new DeviceResourceCache(renderAPI.Factory);

            EcsWorld world = Context.GetSubSystem<WorldSystem>().World;
            entityFilter = world.CreateFilter<Components.Transform, Components.MeshFilter>();
            cameraFilter = world.CreateFilter<Components.Transform, Components.Camera>();

            diffuseCommandList = renderAPI.GenerateCommandList();
            uiCommandList = renderAPI.GenerateCommandList();

            Load();

            ImGuiController.Instance.Init(renderAPI, resolutionWidth, resolutionHeight);

        }

        private void WindowResized(object sender, WindowResizedEventArgs e)
        {
            isWindowResized = true;
            newWindowHeight = e.Height;
            newWindowWidth = e.Width;
        }

        internal override void Tick(double deltaTime)
        {
            if (isWindowResized)
            {
                isWindowResized = false;
                renderAPI.GraphicAPI.ResizeMainWindow(newWindowWidth, newWindowHeight);
                ImGuiController.Instance.WindowResized((int)newWindowWidth, (int)newWindowHeight);
            }

            MainPass();
            ImGuiPass(deltaTime);
            renderAPI.SwapBuffers();
        }

        private void ImGuiPass(double deltaTime)
        {
            if (deltaTime <= 0) deltaTime = 16;
            ImGuiController.Instance.Update((float)(deltaTime / 1000), Context.GetSubSystem<InputSystem>().Snapshot);

            UserInterface.UIController.Instance.Tick();

            uiCommandList.PushDebugGroup("Pass_ImGui");
            uiCommandList.Begin();
            uiCommandList.SetFramebuffer(renderAPI.GraphicAPI.SwapchainFramebuffer);
            uiCommandList.ClearColorTarget(0, RgbaFloat.Black);
            ImGuiController.Instance.Render(uiCommandList);
            uiCommandList.End();
            renderAPI.SubmitCommands(uiCommandList);
            uiCommandList.PopDebugGroup();

        }
    }
}
