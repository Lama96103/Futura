using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Settings
{
    public class EditorSettings
    {
        public Quaternion LastEditorCameraRotation;
        public Vector3 LastEditorCameraPosition;

        public float EditorCameraSpeed = 8f;
        public float EditorCameraMouseSensitivity = 32f;
    }
}
