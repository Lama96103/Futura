using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Resources
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        private const string MetaFileExtension = ".fMeta";
        private DirectoryInfo rootDir;

        private Dictionary<Guid, Asset> loadedAssets = new Dictionary<Guid, Asset>();
        public Dictionary<Guid, Asset>.ValueCollection LoadedAssets { get => loadedAssets.Values; }

        private List<Importer> importers = new List<Importer>();

        private Settings.AssetSettings assetSettings;

        internal void Init(DirectoryInfo rootDirectory)
        {
            this.rootDir = rootDirectory;
            if (!rootDir.Exists) rootDir.Create();

            // Add all available importers
            importers.Add(new Import.MeshImporter());
            assetSettings = Core.Runtime.Instance.Settings.Get<Settings.AssetSettings>();
            CheckFolderForAssets(this.rootDir);
        }


        private void CheckFolderForAssets(DirectoryInfo folder)
        {
            foreach (var dir in folder.GetDirectories()) CheckFolderForAssets(dir);

            foreach(var file in folder.GetFiles())
            {
                if (file.Extension == MetaFileExtension) continue;

                string metaFilePath = file.FullName + MetaFileExtension;
                if (File.Exists(metaFilePath)) LoadAsset(file, new FileInfo(metaFilePath), assetSettings.AutomaticCheckForFileChange);
                else ImportAsset(file);
            }
        }

        private void LoadAsset(FileInfo asset, FileInfo metaFile, bool checkFileChange = true)
        {
            byte[] fileHash = new byte[0];
            if (checkFileChange) fileHash = Helper.CacluateHash(asset);

            using(BinaryReader reader = new BinaryReader(metaFile.OpenRead()))
            {
                byte[] metaHash = reader.ReadBytes(Helper.HashLength);


                if (checkFileChange && !fileHash.SequenceEqual(metaHash))
                {
                    Log.Debug("File hash is not equal, reimporting the asset");
                    reader.Close();
                    metaFile.Delete();
                    ImportAsset(asset);
                    return;
                }

                Guid guid = new Guid(reader.ReadBytes(16));
                AssetType assetType = (AssetType)reader.ReadInt32();

                Asset currentAsset = null;
                switch (assetType)
                {
                    case AssetType.Unkown:
                        break;
                    case AssetType.Texture2d:
                        break;
                    case AssetType.Material:
                        break;
                    case AssetType.Mesh:
                        currentAsset = new Mesh(guid, asset);
                        break;
                }

                currentAsset.Read(reader);
                loadedAssets.Add(guid, currentAsset);
                currentAsset.Load();
            }
        }

        private void ImportAsset(FileInfo file)
        {
            foreach(Importer i in importers)
            {
                if (i.SupportedExtensions.Contains(file.Extension))
                {
                    Asset asset = i.ImportAsset(file);
                    loadedAssets.Add(asset.Identifier, asset);
                    asset.Load();
                    Core.Profiler.Report("Import", file.FullName);

                    FileInfo metaFile = new FileInfo(file.FullName + MetaFileExtension);
                    using(BinaryWriter writer = new BinaryWriter(metaFile.Open(FileMode.OpenOrCreate)))
                    {
                        byte[] fileHash = Helper.CacluateHash(file);
                        writer.Write(fileHash);
                        writer.Write(asset.Identifier.ToByteArray());
                        writer.Write((int)asset.AssetType);
                        asset.Write(writer);
                    }
                    break;
                }
            }   
        }
    }
}
