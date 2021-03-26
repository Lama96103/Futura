using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Settings
{
    public class RenderSettings
    {
        public bool ShowLightGizmo = false;
        public bool ShowMeshBoundsGizmo = false;
        public bool ShowOnlySelectedMeshBoundsGizmo = true;
        public bool ShowCollisionBounds = true;
        public Color WireframeColor = Color.Blue;
        public Color CollisionBounds = Color.Red;
    }
}
