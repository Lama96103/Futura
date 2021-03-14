using Futura.Engine.ECS;
using Futura.Engine.ECS.Components;
using Futura.Engine.ECS.Components.Lights;
using Futura.Engine.Rendering;
using Futura.Engine.Rendering.Gizmo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Core
{
    partial class RenderSystem
    {
        public bool UseEditorCamera { get; set; } = true;
        private TransformGizmo transformGizmo = TransformGizmo.Instance;

        public Dictionary<Color, Entity> EntityColorDictionary { get; private set; } = new Dictionary<Color, Entity>();

        private Vector3 cameraPos;

        public Vector2 Viewport { get => new Vector2(RenderResolutionWidth, RenderResolutionHeight); }

        private void MainPass()
        {
            diffuseCommandList.Begin();

            bool isRendering = UpdateWorldBuffer(diffuseCommandList);

            if (isRendering)
            {
                EntityColorDictionary.Clear();
                DiffusePass(diffuseCommandList);
            }

            diffuseCommandList.End();
            renderAPI.SubmitCommands(diffuseCommandList);
        }

        private bool UpdateWorldBuffer(CommandList commandList)
        {
            commandList.PushDebugGroup("Pass_UpdateBuffer");

            Transform cameraTransform = null;
            Camera camera = null;

            if (UseEditorCamera)
            {
                cameraTransform = EditorCamera.Instance.Transform;
                camera = EditorCamera.Instance.Camera;
            }
            else
            {
                if (cameraFilter.Entities.Count() == 0) return false;
                var cameraEntity = cameraFilter.Entities.ElementAt(0);
                cameraTransform = cameraEntity.GetComponent<Transform>();
                camera = cameraEntity.GetComponent<Camera>();
            }

            camera.UpdatePosition(cameraTransform, (float)RenderResolutionWidth, (float)RenderResolutionHeight);

            WorldBuffer world = new WorldBuffer();
            world.Projection = camera.ProjectionMatrix;
            world.View = camera.ViewMatrix;
            world.ProjectionView = camera.ViewProjectionMatrix;
            world.CameraPosition = cameraTransform.Position;
            world.CameraNear = camera.NearPlane;
            world.CameraFar = camera.FarPlane;

            cameraPos = cameraTransform.Position;

            // commandList.UpdateBuffer(worldBuffer, 0, world);
            renderAPI.GraphicAPI.UpdateBuffer(worldBuffer, 0, world);

            LightingBuffer light = new LightingBuffer();
            if(directionalLightFilter.Entities.Count() == 1)
            {
                var dirLight = directionalLightFilter.Entities.ElementAt(0);

                light.DirectionalLightColor = dirLight.GetComponent<DirectionalLight>().Color.RawData;
                light.DirectionalLightIntensitiy = dirLight.GetComponent<DirectionalLight>().Intensity;
                light.DirectionalLightDirection = dirLight.GetComponent<Transform>().Rotation.ToEulerAngles();
            }
            light.AmbientLightIntensity = 0.01f;

            // commandList.UpdateBuffer(lightingBuffer, 0, light);
            renderAPI.GraphicAPI.UpdateBuffer(lightingBuffer, 0, light);

            commandList.PopDebugGroup();

            return true;
        }

        private void DiffusePass(CommandList commandList)
        {
            commandList.PushDebugGroup("Pass_Diffuse");

            commandList.SetPipeline(diffusePipline);
            commandList.SetFramebuffer(diffuseFramebuffer.Handle);
            commandList.SetGraphicsResourceSet(0, worldSet);
            commandList.SetGraphicsResourceSet(1, modelSet);
            commandList.SetGraphicsResourceSet(2, lightingSet);

            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.ClearColorTarget(1, RgbaFloat.Black);
            commandList.ClearColorTarget(2, RgbaFloat.Black);
            commandList.ClearDepthStencil(RenderAPI.Instance.IsDepthRangeZeroToOne ? 1 : -1, 0);

            foreach (var reference in entityFilter.Entities)
            {
                Transform transform = reference.GetComponent<Transform>();
                MeshFilter filter = reference.GetComponent<MeshFilter>();
                RuntimeComponent runtime = reference.Entity.GetComponent<RuntimeComponent>();

                if (runtime.IsEnabled)
                {
                    if (filter.Mesh == null || filter.Material == null) continue;
                    if (filter.Mesh.IsLoaded == false) continue;

                    byte[] colorData = BitConverter.GetBytes(reference.Entity.ID);

                    ModelBuffer model = new ModelBuffer()
                    {
                        Transform = transform.LocalMatrix,
                        ColorIdentifier = new System.Numerics.Vector4(colorData[0]/255f, colorData[1] / 255f, colorData[2] / 255f, 1),
                        DiffuseColor = filter.UseModelColor ? filter.ModelDiffuseColor.RawData : filter.Material.DiffuseColor.RawData
                    };
                    commandList.UpdateBuffer(modelBuffer, 0, model);
                    filter.Mesh.Renderable.Draw(commandList);


                    EntityColorDictionary.Add(new Color(model.ColorIdentifier), reference.Entity);
                }
            }

            // Draw gizmo
            commandList.SetPipeline(gizmoPipline);
            transformGizmo.Tick(commandList, modelBuffer, cameraPos);
          

            // TODO Do this only if enabled
            commandList.CopyTexture(DiffuseFrameBuffer.ColorTextures[1].Handle, SelectionTexture.Handle);

            commandList.PopDebugGroup();
        }

   
     
    }
}
