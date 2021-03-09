﻿using Futura.ECS;
using Futura.Engine.Components;
using Futura.Engine.ECS;
using Futura.Engine.Rendering;
using Futura.Engine.Rendering.Gizmo;
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
        public bool UseEditorCamera { get; set; } = true;
        public TransformGizmo TransformGizmo { get; init; } = new TransformGizmo();

        public Dictionary<Color, Entity> EntityColorDictionary { get; private set; } = new Dictionary<Color, Entity>();

        private void MainPass()
        {
            if (!UpdateWorldBuffer()) return;

            EntityColorDictionary.Clear();
            DiffusePass(diffuseCommandList);
        }

        private bool UpdateWorldBuffer()
        {
            Transform transform = null;
            Camera camera = null;

            if (UseEditorCamera)
            {
                transform = EditorCamera.Instance.Transform;
                camera = EditorCamera.Instance.Camera;
            }
            else
            {
                if (cameraFilter.Entities.Count() == 0) return false;
                var cameraEntity = cameraFilter.Entities.ElementAt(0);
                transform = cameraEntity.GetComponent<Transform>();
                camera = cameraEntity.GetComponent<Camera>();
            }

            camera.UpdatePosition(transform, (float)renderResolutionWidth, (float)renderResolutionHeight);

            WorldBuffer world = new WorldBuffer();
            world.Projection = camera.ProjectionMatrix;
            world.View = camera.ViewMatrix;
            world.ProjectionView = camera.ViewProjectionMatrix;
            world.CameraPosition = transform.Position;
            world.CameraNear = camera.NearPlane;
            world.CameraFar = camera.FarPlane;

            renderAPI.GraphicAPI.UpdateBuffer(worldBuffer, 0, world);

            return true;
        }

        private void DiffusePass(CommandList commandList)
        {
            commandList.Begin();
            commandList.PushDebugGroup("Pass_Diffuse");

            commandList.SetPipeline(diffusePipline);
            commandList.SetFramebuffer(diffuseFramebuffer.Handle);
            commandList.SetGraphicsResourceSet(0, worldSet);
            commandList.SetGraphicsResourceSet(1, modelSet);

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
                        ColorIdentifier = new System.Numerics.Vector4(colorData[0]/255f, colorData[1] / 255f, colorData[2] / 255f, 1)
                    };
                    commandList.UpdateBuffer(modelBuffer, 0, model);
                    filter.Mesh.Renderable.Draw(commandList);


                    EntityColorDictionary.Add(new Color(model.ColorIdentifier), reference.Entity);
                }
            }

            // TODO Do this only if enabled
            commandList.CopyTexture(DiffuseFrameBuffer.ColorTextures[1].Handle, SelectionTexture.Handle);

            commandList.PopDebugGroup();
            commandList.End();
            renderAPI.SubmitCommands(commandList);
        }

   
     
    }
}
