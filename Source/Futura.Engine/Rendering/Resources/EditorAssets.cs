using Futura.Engine;
using Futura.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Futura.Rendering.Resources
{
    public static class EditorAssets
    {
        public static Guid DiffuseShaderGuid = new Guid("983f3ce8-7a79-493a-a6a8-9b2b08df4067");

        #region Built-In Shader Data
        public static readonly byte[] ImGuiVertex = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.ImGui.ImGui.vert.spv");
        public static readonly byte[] ImGuiFragment = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.ImGui.ImGui.frag.spv");

        public static readonly byte[] DiffuseVertex = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.Diffuse.Diffuse.vert.spv");
        public static readonly byte[] DiffuseFragment = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.Diffuse.Diffuse.frag.spv");

        public static readonly byte[] SelectionVertex = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.Selection.Selection.vert.spv");
        public static readonly byte[] SelectionFragment = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.Selection.Selection.frag.spv");

        public static readonly byte[] DebugVertex = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.DebugShader.DebugShader.vert.spv");
        public static readonly byte[] DebugFragment = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.DebugShader.DebugShader.frag.spv");
        #endregion

 

        #region Built-In Texture
        public static readonly byte[] ColorTexture = Helper.GetEmbeddedRessource("Futura.Engine.Rendering.Resources.Textures.Colors.asset");
        public static readonly Guid ColorTextureGuid = Guid.NewGuid();
        #endregion

        //public static RenderAble GetBoxRenderAble(float index = 1)
        //{
        //    return MeshBuilder.Instance.BuildBox(index);

        //}

        //public static void Load()
        //{   
        //    var meta = new AssetMetaData("Arrow", EditorAssets.ArrowModelGuid, "Built-In", AssetType.VoxelMesh);
        //    VoxelMesh mesh = new VoxelMesh(meta);
        //    mesh.SetSerializationData(EditorAssets.ArrowModel);
        //    mesh.Load();
        //    AssetController.Instance.AddLoadedAsset(meta.Guid, mesh);

        //    meta = new AssetMetaData("Color Palette", EditorAssets.ColorTextureGuid, "Built-In", AssetType.Texture);
        //    TextureAsset texture = new TextureAsset(EditorAssets.ColorTexture, meta);
        //    texture.Load();
        //    AssetController.Instance.AddLoadedAsset(meta.Guid, texture);
        //}
    }
}
