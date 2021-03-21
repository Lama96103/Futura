using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
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

    [StructLayout(LayoutKind.Sequential)]
    struct ModelBuffer
    {
        public Matrix4x4 Transform;
        public Vector4 ColorIdentifier;
        public Vector4 DiffuseColor;

        public float IsLightingEnabled;
        private float _padding0;
        private float _padding1;
        private float _padding2;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct LightingBuffer
    {
        public Vector3 DirectionalLightColor;
        public float DirectionalLightIntensitiy;

        public Vector3 DirectionalLightDirection;
        public float AmbientLightIntensity;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct PointLightInfo
    {
        public Vector3 Position;
        public float Intensity;
        public Vector3 Color;
        public float Range;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct PointLightsInfo
    {
        public PointLightInfo[] PointLights;
        public int NumActiveLights;
        public float _padding0;
        public float _padding1;
        public float _padding2;


        public Blittable GetBlittable()
        {
            return new Blittable
            {
                NumActiveLights = NumActiveLights,
                PointLights0 = PointLights[0],
                PointLights1 = PointLights[1],
                PointLights2 = PointLights[2],
                PointLights3 = PointLights[3],
            };
        }

        public struct Blittable
        {
            public PointLightInfo PointLights0;
            public PointLightInfo PointLights1;
            public PointLightInfo PointLights2;
            public PointLightInfo PointLights3;

            public int NumActiveLights;
            public float _padding0;
            public float _padding1;
            public float _padding2;
        }
    }
}

