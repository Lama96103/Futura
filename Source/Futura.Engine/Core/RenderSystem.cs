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
        private CommandList commandList;
        private DeviceResourceCache deviceResourceCache;

        private int resolutionWidth;
        private int resolutionHeight;

        private ECS.EcsFilter entityFilter;
        private ECS.EcsFilter cameraFilter;

        private CommandList diffuseCommandList;

        internal override void Init()
        {
            renderAPI = new RenderAPI(this.Context, Window.Instance);
            resolutionWidth = Window.Instance.Width;
            resolutionHeight = Window.Instance.Height;

            commandList = renderAPI.GenerateCommandList();
            deviceResourceCache = new DeviceResourceCache(renderAPI.Factory);

            ECS.EcsWorld world = Context.GetSubSystem<WorldSystem>().World;
            entityFilter = new ECS.EcsFilter(world, new ECS.IComponent[] { new Components.Transform(), new Components.MeshFilter() });
            cameraFilter = new ECS.EcsFilter(world, new ECS.IComponent[] { new Components.Transform(), new Components.Camera() });

            diffuseCommandList = renderAPI.GenerateCommandList();

            Load();

        }

        internal override void Tick(double deltaTime)
        {
            MainPass();
            renderAPI.SwapBuffers();
        }
    }
}
