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

        private ECS.EcsFilter entityFilter;
        private ECS.EcsFilter cameraFilter;

        private CommandList diffuseCommandList;
        private CommandList uiCommandList;

        internal override void Init()
        {
            renderAPI = new RenderAPI(this.Context, Window.Instance);
            resolutionWidth = Window.Instance.Width;
            resolutionHeight = Window.Instance.Height;

            deviceResourceCache = new DeviceResourceCache(renderAPI.Factory);

            ECS.EcsWorld world = Context.GetSubSystem<WorldSystem>().World;
            entityFilter = new ECS.EcsFilter(world, new ECS.IComponent[] { new Components.Transform(), new Components.MeshFilter() });
            cameraFilter = new ECS.EcsFilter(world, new ECS.IComponent[] { new Components.Transform(), new Components.Camera() });

            diffuseCommandList = renderAPI.GenerateCommandList();
            uiCommandList = renderAPI.GenerateCommandList();

            Load();

            ImGuiController.Instance.Init(renderAPI, resolutionWidth, resolutionHeight);

        }

        internal override void Tick(double deltaTime)
        {
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
