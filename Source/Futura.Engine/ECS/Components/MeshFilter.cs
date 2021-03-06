﻿using Futura.Engine.Rendering;
using Futura.Engine.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.ECS.Components
{
    public class MeshFilter : IComponent
    {
        public Mesh Mesh;
        public Material Material;

        public bool UseModelColor = false;
        public Color ModelDiffuseColor = Color.Black;

    }
}
