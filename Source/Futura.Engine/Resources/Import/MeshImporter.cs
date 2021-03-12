using Assimp;
using Assimp.Configs;
using Futura.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Resources.Import
{
    internal class MeshImporter : Importer
    {
        internal override AssetType AssetType => AssetType.Mesh;

        internal override string[] SupportedExtensions => new string[] { ".fbx", ".obj"};

        internal override Asset ImportAsset(FileInfo file, Guid guid)
        {
            if (!file.Exists)
            {
                Log.Error("File does not exist " + file.FullName);
                return null;
            }

            AssimpContext importer = new AssimpContext();
            //importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            //importer.SetConfig(new Assimp.Configs.)
            Scene scene = importer.ImportFile(file.FullName, PostProcessSteps.Triangulate);

            if (!scene.HasMeshes)
            {
                Log.Error("Assimp: Scene has no meshes");
                return null;
            }

            if (scene.MeshCount > 1) Log.Warn("Assimp: Scene has more then 1 mesh, only the first will be imported");

            Assimp.Mesh assimpMesh = scene.Meshes[0];

            if (!assimpMesh.HasVertices)
            {
                Log.Error("Assimp: Mesh does not have vertices");
                return null;
            }
            int[] assimpIndices = assimpMesh.GetIndices();

            Vector3 boundsMin = new Vector3();
            Vector3 boundsMax = new Vector3();

            uint[] indices = new uint[assimpIndices.Length];
            Vertex[] vertices = new Vertex[assimpMesh.VertexCount];

            Vector3[] vertexPositions = new Vector3[assimpMesh.VertexCount];
            Vector3[] vertexNormals = new Vector3[assimpMesh.VertexCount];
            Vector2[] vertexUVs = new Vector2[assimpMesh.VertexCount];

            for (int i = 0; i < assimpMesh.VertexCount; i++)
            {
                var v = assimpMesh.Vertices[i];
                vertexPositions[i] = new Vector3(v.X, v.Y, v.Z);
                boundsMin = Vector3.Min(boundsMin, vertexPositions[i]);
                boundsMax = Vector3.Max(boundsMax, vertexPositions[i]);
            }

            if (assimpMesh.HasNormals)
                for (int i = 0; i < assimpMesh.VertexCount; i++)
                {
                    var v = assimpMesh.Normals[i];
                    vertexNormals[i] = new Vector3(v.X, v.Y, v.Z);
                }
            else
                Log.Warn("Assimp: Mesh does not have normals");

            if (assimpMesh.HasTextureCoords(0))
                for (int i = 0; i < assimpMesh.VertexCount; i++)
                {
                    var v = assimpMesh.TextureCoordinateChannels[0][i];
                    vertexUVs[i] = new Vector2(v.X, v.Y);
                }
            else
                Log.Warn("Assimp: Mesh does not have texture coordinates");



            for (int i = 0; i < assimpMesh.VertexCount; i++)
            {
                Vertex v = new Vertex(vertexPositions[i], vertexNormals[i], vertexUVs[i]);
                vertices[i] = v;
            }

            for (int i = 0; i < assimpIndices.Length; i++)
            {
                indices[i] = (uint)assimpIndices[i];
            }

            return new Mesh(file, guid, vertices, indices, new Core.Bounds(boundsMin, boundsMax, false));
        }
    }
}
