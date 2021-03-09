using Futura.Engine;
using Futura.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Futura.Rendering.Resources
{
    public static class EditorAssets
    {
        #region Built-In Shader Data
        public static readonly byte[] ImGuiVertex = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.ImGui.ImGui.vert.spv");
        public static readonly byte[] ImGuiFragment = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.ImGui.ImGui.frag.spv");

        public static readonly byte[] DiffuseVertex = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.Diffuse.Diffuse.vert.spv");
        public static readonly byte[] DiffuseFragment = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.Diffuse.Diffuse.frag.spv");

        public static readonly byte[] DebugVertex = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.DebugShader.DebugShader.vert.spv");
        public static readonly byte[] DebugFragment = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.DebugShader.DebugShader.frag.spv");
        #endregion

 

        #region Built-In Texture

        #endregion
    }
}
