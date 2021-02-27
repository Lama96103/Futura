using Futura.Engine.Components;
using Futura.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Core
{
    partial class RenderSystem
    {


        private void MainPass()
        {
            if (cameraFilter.Entities.Count() == 0) return;
            UpdateWorldBuffer();


            DiffusePass(diffuseCommandList);
        }

        private void UpdateWorldBuffer()
        {
            var cameraEntity = cameraFilter.Entities.ElementAt(0);
            Transform transform = cameraEntity.Components[0] as Transform;
            Camera camera = cameraEntity.Components[1] as Camera;

            WorldBuffer world = new WorldBuffer();
            world.Projection = camera.GetProjection(transform);
            world.View = camera.GetView(transform);
            world.ProjectionView = camera.GetViewProjectionMatrix(transform);
            world.CameraPosition = transform.Position;

            renderAPI.GraphicAPI.UpdateBuffer(worldBuffer, 0, world);
        }

        private void DiffusePass(CommandList commandList)
        {
            commandList.Begin();
            commandList.PushDebugGroup("Pass_Diffuse");

            commandList.SetPipeline(diffusePipline);
            commandList.SetFramebuffer(renderAPI.GraphicAPI.SwapchainFramebuffer);
            commandList.SetGraphicsResourceSet(0, worldSet);
            commandList.SetGraphicsResourceSet(1, modelSet);

            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.ClearDepthStencil(RenderAPI.IsDepthRangeZeroToOne ? 1 : -1, 0);


            foreach (var reference in entityFilter.Entities)
            {
                Transform transform = reference.GetComponent<Transform>();
                MeshFilter filter = reference.GetComponent<MeshFilter>();


                ModelBuffer model = new ModelBuffer()
                {
                    Transform = transform.CalculateModelMatrix(),
                    Color = new System.Numerics.Vector4(1.0f)
                };
                commandList.UpdateBuffer(modelBuffer, 0, model);
                testRenderAble.Draw(commandList);
            }


            commandList.PopDebugGroup();
            commandList.End();
            renderAPI.SubmitCommands(commandList);
        }

     
    }
}
