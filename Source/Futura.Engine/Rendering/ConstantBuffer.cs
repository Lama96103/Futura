using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Rendering
{
    struct WorldBuffer
    {
        public Matrix4x4 Projection;
        public Matrix4x4 View;
        public Matrix4x4 ProjectionView;

        public Vector3 CameraPosition;
        public float CameraNear;
        public float CameraFar;
        private Vector3 padding;
    }

    struct ModelBuffer
    {
        public Matrix4x4 Transform;
        public Vector4 ColorIdentifier;
        public Vector4 DiffuseColor;
    }
}
