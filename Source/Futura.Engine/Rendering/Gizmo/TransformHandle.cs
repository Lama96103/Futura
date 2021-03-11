using Futura.Engine.Components;
using Futura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Futura.ECS;

namespace Futura.Engine.Rendering.Gizmo
{
    abstract class TransformHandle
    {
        protected Renderable renderable;

        public abstract void Init(Veldrid.ResourceFactory factory);


        public void Tick(CommandList commandList, Entity entity, Veldrid.DeviceBuffer modelBuffer, Vector3 cameraPos)
        {
            Transform transform = entity.GetComponent<Transform>();


            float distanceToCamera = Vector3.Distance(transform.Position, cameraPos);

            Vector3 scale = new Vector3(distanceToCamera / (1 - 0.015f));
            scale *= 0.04f;

            // X Axis
            Vector3 position = transform.Position;
            Vector3 rotation = new Vector3(0, 0, -90);
            Vector4 color = TransformGizmo.ColorAxisX.RawData;

            Matrix4x4 matrix = Transform.CalculateModelMatrix(position, rotation.FromEulerAngles(), scale);

            ModelBuffer model = new ModelBuffer()
            {
                Transform = matrix,
                ColorIdentifier = color,
                DiffuseColor = color
            };
            commandList.UpdateBuffer(modelBuffer, 0, model);
            renderable.Draw(commandList);

            // Y Axis
            rotation = new Vector3(0, 90, 0);
            color = TransformGizmo.ColorAxisY.RawData;

            matrix = Transform.CalculateModelMatrix(position, rotation.FromEulerAngles(), scale);

            model = new ModelBuffer()
            {
                Transform = matrix,
                ColorIdentifier = color,
                DiffuseColor = color
            };
            commandList.UpdateBuffer(modelBuffer, 0, model);
            renderable.Draw(commandList);

            // Z Axis
            rotation = new Vector3(90, 0, 0);
            color = TransformGizmo.ColorAxisZ.RawData;

            matrix = Transform.CalculateModelMatrix(position, rotation.FromEulerAngles(), scale);

            model = new ModelBuffer()
            {
                Transform = matrix,
                ColorIdentifier = color,
                DiffuseColor = color
            };
            commandList.UpdateBuffer(modelBuffer, 0, model);
            renderable.Draw(commandList);

            Core.Profiler.Report(Core.Profiler.StatisticIndicator.DrawCall, 3);
            Core.Profiler.Report(Core.Profiler.StatisticIndicator.Vertex, (int)renderable.Vertices * 3);
        }
    }
}
