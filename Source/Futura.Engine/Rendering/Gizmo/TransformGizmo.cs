using Futura.ECS;
using Futura.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Futura.Engine.Rendering.Gizmo
{
    class TransformGizmo : Singleton<TransformGizmo>
    {
        public static readonly Color ColorAxisX = new Color(1, 0, 0, 1);
        public static readonly Color ColorAxisY = new Color(0, 1, 0, 1);
        public static readonly Color ColorAxisZ = new Color(0, 0, 1, 1);
        public static readonly Color ColorAxisHoover = new Color(0.75f, 0.75f, 0.75f, 1);

        public static readonly Color ColorAxisXId = new Color(1, 0, 0, 0);
        public static readonly Color ColorAxisYId = new Color(0, 1, 0, 0);
        public static readonly Color ColorAxisZId = new Color(0, 0, 1, 0);


        private Transformation currentTransformation = Transformation.Move;

        private Entity currentSelectedEntity;
        private Vector3 currentEditAxis = Vector3.Zero;
        private Vector3 lastPos = Vector3.Zero;
        private Vector2 viewportOffset = Vector2.Zero;

        private TransformPositionHandle positionHandle;

        private Vector3 currentHoverAxis = Vector3.Zero;

        public void Init(ResourceFactory factory)
        {
            positionHandle = new TransformPositionHandle();
            positionHandle.Init(factory);

            Core.RuntimeHelper.Instance.EntitySelectionChanged += Instance_EntitySelectionChanged;
        }

        private void Instance_EntitySelectionChanged(object sender, Core.EntitySelectionChangedEventArgs e)
        {
            currentSelectedEntity = e.Entity;
        }

        public void Tick(CommandList commandList, Veldrid.DeviceBuffer modelBuffer, Vector3 cameraPos)
        {
            if (currentSelectedEntity == null) return;

            switch (currentTransformation)
            {
                case Transformation.Move:
                    if (currentEditAxis != Vector3.Zero) ApplyTransform();
                    positionHandle.Tick(commandList, currentSelectedEntity, modelBuffer, cameraPos, currentHoverAxis);
                    break;
                case Transformation.Rotate:
                    break;
                case Transformation.Scale:
                    break;
                default:
                    break;
            }
        }

        private void ApplyTransform()
        {
            float speed = 12;

            Vector2 mousePos = Input.MousePosition- viewportOffset;

            Vector3 currentPos = EditorCamera.Instance.Camera.Unproject(mousePos);

            Vector3 delta = currentPos - lastPos;
            float deltaXYZ = delta.Length();

            float deltaX = deltaXYZ * Math.Sign(delta.X) * speed;
            float deltaY = deltaXYZ * Math.Sign(delta.Y) * speed;
            float deltaZ = deltaXYZ * Math.Sign(delta.Z) * speed;


            currentSelectedEntity.GetComponent<Components.Transform>().Translate(new Vector3(deltaX, deltaY, deltaZ) * currentEditAxis);

            lastPos = currentPos;
        }

        public void StartEditing(Vector3 axis, Vector2 mousePos, Vector2 viewportSize)
        {
            currentEditAxis = axis;

            lastPos = EditorCamera.Instance.Camera.Unproject(mousePos) * currentEditAxis;
            this.viewportOffset = viewportSize;
        }

        public void EndEditing()
        {
            currentEditAxis = Vector3.Zero;
        }

        public void SetHooverAxis(Vector3 axis)
        {
            currentHoverAxis = axis;
        }


        private enum Transformation { Move, Rotate, Scale }
    }
}
