﻿using Futura.Engine.Components;
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


        private void MainPass()
        {
            if (!UpdateWorldBuffer()) return;

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

            Random random = new Random();
            foreach (var reference in entityFilter.Entities)
            {
                Transform transform = reference.GetComponent<Transform>();
                MeshFilter filter = reference.GetComponent<MeshFilter>();
                RuntimeComponent runtime = reference.Entity.GetComponent<RuntimeComponent>();

                if (runtime.IsEnabled)
                {
                    if (filter.Mesh == null || filter.Material == null) continue;
                    if (filter.Mesh.IsLoaded == false) continue;

                    
                    ModelBuffer model = new ModelBuffer()
                    {
                        Transform = transform.LocalMatrix,
                        Color = new System.Numerics.Vector4((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1.0f)
                    };
                    commandList.UpdateBuffer(modelBuffer, 0, model);
                    filter.Mesh.Renderable.Draw(commandList);
                }
            }


            commandList.PopDebugGroup();
            commandList.End();
            renderAPI.SubmitCommands(commandList);
        }

   
     
    }
}
