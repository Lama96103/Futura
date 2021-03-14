using Futura.Engine.ECS;
using Futura.Engine.ECS.Components;
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
        private EcsFilter directionalLightFilter;

        private CommandList diffuseCommandList;
        private CommandList uiCommandList;

        private Tuple<uint, uint> mainWindowResized = null;
        private Tuple<uint, uint> renderResolutionResized = null;

        internal override void Init()
        {
            renderAPI = new RenderAPI(this.Context, Window.Instance);
            resolutionWidth = Window.Instance.Width;
            resolutionHeight = Window.Instance.Height;
            Window.Instance.WindowResized += WindowResized;

            deviceResourceCache = new DeviceResourceCache(renderAPI.Factory);

            EcsWorld world = Context.GetSubSystem<WorldSystem>().World;
            entityFilter = world.CreateFilter<Transform, MeshFilter>();
            cameraFilter = world.CreateFilter<Transform, Camera>();
            directionalLightFilter = world.CreateFilter<Transform, ECS.Components.Lights.DirectionalLight>();

            diffuseCommandList = renderAPI.GenerateCommandList();
            uiCommandList = renderAPI.GenerateCommandList();

            Load();

            ImGuiController.Instance.Init(renderAPI, resolutionWidth, resolutionHeight);
        }

        private void WindowResized(object sender, WindowResizedEventArgs e)
        {
            mainWindowResized = new Tuple<uint, uint>(e.Width, e.Height);
        }

        public void ResizeRenderResolution(uint width, uint height)
        {
            renderResolutionResized = new Tuple<uint, uint>(width, height);
        }

        internal override void Tick(double deltaTime)
        {
            if (mainWindowResized != null)
            {
                renderAPI.GraphicAPI.ResizeMainWindow(mainWindowResized.Item1, mainWindowResized.Item2);
                ImGuiController.Instance.WindowResized((int)mainWindowResized.Item1, (int)mainWindowResized.Item2);
                ImGuiController.Instance.ClearCachedImageResources();
                mainWindowResized = null;
            }

            if(renderResolutionResized != null)
            {
                RecreateRenderResources(renderResolutionResized.Item1, renderResolutionResized.Item2);
                ImGuiController.Instance.ClearCachedImageResources();
                renderResolutionResized = null;
            }

            if(entityFilter.IsDead || cameraFilter.IsDead)
            {
                EcsWorld world = Context.GetSubSystem<WorldSystem>().World;
                entityFilter = world.CreateFilter<Transform, MeshFilter>();
                cameraFilter = world.CreateFilter<Transform, Camera>();
                directionalLightFilter = world.CreateFilter<Transform, ECS.Components.Lights.DirectionalLight>();
            }

            MainPass();
            ImGuiPass(deltaTime);

            renderAPI.SwapBuffers();
            renderAPI.WaitForIdle();
        }

        private void ImGuiPass(double deltaTime)
        {
            if (deltaTime <= 0) deltaTime = 16;
            ImGuiController.Instance.Update((float)(deltaTime / 1000), Context.GetSubSystem<InputSystem>().Snapshot);

            UserInterface.UIController.Instance.Tick();

            uiCommandList.Begin();
            uiCommandList.PushDebugGroup("Pass_ImGui");

            uiCommandList.SetFramebuffer(renderAPI.GraphicAPI.SwapchainFramebuffer);
            uiCommandList.ClearColorTarget(0, RgbaFloat.Black);
            ImGuiController.Instance.Render(uiCommandList);

            uiCommandList.PopDebugGroup();
            uiCommandList.End();
            renderAPI.SubmitCommands(uiCommandList);

        }
    }
}
