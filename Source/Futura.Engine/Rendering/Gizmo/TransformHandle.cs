using Futura.Engine.Components;
using Futura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Rendering.Gizmo
{
    abstract class TransformHandle
    {
        protected Renderable renderable;

        public abstract void Init(Veldrid.ResourceFactory factory);

        public void Tick(CommandList commandList, Transform transform, Veldrid.DeviceBuffer modelBuffer, Vector3 cameraPos)
        {
            float distanceToCamera = Vector3.Distance(transform.Position, cameraPos);

            // X Axis
            Vector3 position = transform.Position + new Vector3(2.5f * 0.2f, 0, 0);
            Vector3 scale = new Vector3(0.05f * distanceToCamera);
            Vector3 rotation = new Vector3(0, 0, -90);
            Vector4 color = new System.Numerics.Vector4(1, 0, 0, 1);

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
            position = transform.Position + new Vector3(0, 2.5f * 0.2f, 0);
            rotation = new Vector3(0, 90, 0);
            color = new Vector4(0, 1, 0, 1);

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
            position = transform.Position + new Vector3(0, 0, 2.5f * 0.2f);
            rotation = new Vector3(90, 0, 0);
            color = new Vector4(0, 0, 1, 1);

            matrix = Transform.CalculateModelMatrix(position, rotation.FromEulerAngles(), scale);

            model = new ModelBuffer()
            {
                Transform = matrix,
                ColorIdentifier = color,
                DiffuseColor = color
            };
            commandList.UpdateBuffer(modelBuffer, 0, model);
            renderable.Draw(commandList);
        }
    }
}
