using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
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

        private float padding01;
        private float padding02;
        private float padding03;
    }

    struct ModelBuffer
    {
        public Matrix4x4 Transform;
        public Vector4 ColorIdentifier;
        public Vector4 DiffuseColor;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct LightingBuffer
    {
        public Vector4 DirectionalLightColor;

        public Vector3 DirectionalLightDirection;
        public float DirectionalLightIntensitiy;

        private Vector3 padding01;
        public float AmbientLightIntensity;

    }
}
