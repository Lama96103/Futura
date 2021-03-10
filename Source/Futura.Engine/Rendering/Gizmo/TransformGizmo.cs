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
    class TransformGizmo
    {
        private Transformation currentTransformation = Transformation.Move;

        private TransformPositionHandle positionHandle;

        public void Init(ResourceFactory factory)
        {
            positionHandle = new TransformPositionHandle();
            positionHandle.Init(factory);
        }

        public void Tick(CommandList commandList, Transform transform, Veldrid.DeviceBuffer modelBuffer, Vector3 cameraPos)
        {
            switch (currentTransformation)
            {
                case Transformation.Move:
                    positionHandle.Tick(commandList, transform, modelBuffer, cameraPos);
                    break;
                case Transformation.Rotate:
                    break;
                case Transformation.Scale:
                    break;
                default:
                    break;
            }
        }


        private enum Transformation { Move, Rotate, Scale }
    }
}
