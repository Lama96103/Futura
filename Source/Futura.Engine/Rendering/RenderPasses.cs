using Futura.Engine.ECS;
using Futura.Engine.ECS.Components;
using Futura.Engine.ECS.Components.Lights;
using Futura.Engine.Physics.ECS;
using Futura.Engine.Rendering;
using Futura.Engine.Rendering.Gizmo;
using Futura.Engine.Settings;
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

        private LightingSettings lightingSettings;
        public RenderSettings renderSettings;

        private void MainPass()
        {
            diffuseCommandList.Begin();

            lightingSettings = Runtime.Instance.Settings.Get<LightingSettings>();
            renderSettings = Runtime.Instance.Settings.Get<RenderSettings>();

            bool isRendering = UpdateWorldBuffer(diffuseCommandList);

            if (isRendering)
            {
                EntityColorDictionary.Clear();
                DiffusePass(diffuseCommandList);
                DebugPass(diffuseCommandList);
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

            commandList.UpdateBuffer(worldBuffer, 0, world);

            // Update DirectionLight Information
            LightingBuffer light = new LightingBuffer();
            if(directionalLightFilter.Entities.Count() == 1)
            {
                var dirLight = directionalLightFilter.Entities.ElementAt(0);

                light.DirectionalLightColor = dirLight.GetComponent<DirectionalLight>().Color.ToVector3();
                light.DirectionalLightIntensitiy = dirLight.GetComponent<DirectionalLight>().Intensity;
                light.DirectionalLightDirection = dirLight.GetComponent<Transform>().Rotation.ToEulerAngles();
            }
            light.AmbientLightIntensity = lightingSettings.AmbientLightIntensity;

            commandList.UpdateBuffer(lightingBuffer, 0, light);

            /// Update PointLight Information;
            if (pointLightFilter.Entities.Count() > 0)
            {
                PointLightsInfo pointLightsInfo = new PointLightsInfo()
                {
                    PointLights = new PointLightInfo[4],
                    NumActiveLights = pointLightFilter.Entities.Count() >= 4 ? 3 : pointLightFilter.Entities.Count()
                };

                for (int i = 0; i < pointLightFilter.Entities.Count(); i++)
                {
                    if (i >= 4) break;
                    pointLightsInfo.PointLights[i].Position = pointLightFilter.Entities.ElementAt(i).GetComponent<Transform>().Position;
                    pointLightsInfo.PointLights[i].Color = pointLightFilter.Entities.ElementAt(i).GetComponent<PointLight>().Color.ToVector3();
                    pointLightsInfo.PointLights[i].Range = pointLightFilter.Entities.ElementAt(i).GetComponent<PointLight>().Range;
                    pointLightsInfo.PointLights[i].Intensity = pointLightFilter.Entities.ElementAt(i).GetComponent<PointLight>().Intensity;
                }

                // renderAPI.GraphicAPI.UpdateBuffer(pointLightBuffer, 0, pointLightsInfo);
                commandList.UpdateBuffer(pointLightBuffer, 0, pointLightsInfo.GetBlittable());
            }

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

            commandList.ClearColorTarget(0, lightingSettings.BackgroundColor.ToRgbaFloat());
            commandList.ClearColorTarget(1, RgbaFloat.Black);
            commandList.ClearColorTarget(2, RgbaFloat.Black);
            commandList.ClearDepthStencil(RenderAPI.Instance.IsDepthRangeZeroToOne ? 1 : -1, 0);

            foreach (var reference in entityFilter.Entities)
            {
                Transform transform = reference.GetComponent<Transform>();
                MeshFilter filter = reference.GetComponent<MeshFilter>();
                RuntimeComponent runtime = reference.Entity.Get<RuntimeComponent>();

                if (runtime.IsEnabled)
                {
                    if (filter.Mesh == null || filter.Material == null) continue;
                    if (filter.Mesh.IsLoaded == false) continue;

                    byte[] colorData = BitConverter.GetBytes(reference.Entity.ID);

                    ModelBuffer model = new ModelBuffer()
                    {
                        Transform = transform.LocalMatrix,
                        ColorIdentifier = new System.Numerics.Vector4(colorData[0] / 255f, colorData[1] / 255f, colorData[2] / 255f, 1),
                        DiffuseColor = filter.UseModelColor ? filter.ModelDiffuseColor.RawData : filter.Material.DiffuseColor.RawData,
                        IsLightingEnabled = 1.0f
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

        private void DebugPass(CommandList commandList)
        {
            commandList.PushDebugGroup("Pass_Debug");
            commandList.SetPipeline(wireframePipline);


            if (renderSettings.ShowLightGizmo)
            {
                foreach (var reference in pointLightFilter.Entities)
                {
                    Transform transform = reference.GetComponent<Transform>();
                    PointLight light = reference.GetComponent<PointLight>();
                    RuntimeComponent runtime = reference.Entity.Get<RuntimeComponent>();

                    if (runtime.IsEnabled && runtime.IsSelected)
                    {
                        ModelBuffer model = new ModelBuffer()
                        {
                            Transform = Transform.CalculateModelMatrix(transform.Position, Quaternion.Identity, new Vector3(light.Range)),
                            DiffuseColor = renderSettings.WireframeColor.RawData,
                            IsLightingEnabled = 0.0f
                        };
                        commandList.UpdateBuffer(modelBuffer, 0, model);
                        debugSphere.Draw(commandList);
                    }
                }
            }

            if (renderSettings.ShowMeshBoundsGizmo)
            {
                foreach (var reference in entityFilter.Entities)
                {
                    Transform transform = reference.GetComponent<Transform>();
                    MeshFilter filter = reference.GetComponent<MeshFilter>();
                    RuntimeComponent runtime = reference.Entity.Get<RuntimeComponent>();

                    if (runtime.IsEnabled && (runtime.IsSelected || !renderSettings.ShowOnlySelectedMeshBoundsGizmo))
                    {
                        if (filter.Mesh == null || filter.Material == null) continue;
                        if (filter.Mesh.IsLoaded == false) continue;

                        Bounds b = filter.Mesh.Bounds;
                        if (b == default(Bounds))
                        {
                            filter.Mesh.RecaculateBounds();
                            b = filter.Mesh.Bounds;
                        }

                        ModelBuffer model = new ModelBuffer()
                        {
                            Transform = Transform.CalculateModelMatrix(transform.Position + b.Center, transform.Rotation, b.Extends),
                            DiffuseColor = renderSettings.WireframeColor.RawData,
                            IsLightingEnabled = 0.0f
                        };
                        commandList.UpdateBuffer(modelBuffer, 0, model);
                        debugBox.Draw(commandList);
                    }
                }
            }

            if (renderSettings.ShowCollisionBounds)
            {
                foreach (var reference in boxCollider.Entities)
                {
                    Transform transform = reference.GetComponent<Transform>();
                    BoxCollider collider = reference.GetComponent<BoxCollider>();
                    RuntimeComponent runtime = reference.Entity.Get<RuntimeComponent>();

                    if (runtime.IsEnabled)
                    {
                        ModelBuffer model = new ModelBuffer()
                        {
                            Transform = Transform.CalculateModelMatrix(transform.Position, transform.Rotation, collider.Size),
                            DiffuseColor = renderSettings.CollisionBounds.RawData,
                            IsLightingEnabled = 0.0f
                        };
                        commandList.UpdateBuffer(modelBuffer, 0, model);
                        debugBox.Draw(commandList);
                    }
                }
            }


            commandList.PopDebugGroup();
        }

   
     
    }
}
