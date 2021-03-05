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
        private const string AssetFileExtension = ".fAsset";
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

                if(file.Extension == AssetFileExtension)
                {
                    ReadFuturaAsset(file);
                }
                else
                {
                    string metaFilePath = file.FullName + MetaFileExtension;
                    if (File.Exists(metaFilePath)) LoadAsset(file, new FileInfo(metaFilePath), assetSettings.AutomaticCheckForFileChange);
                    else ImportAsset(file);
                }

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

        private void ReadFuturaAsset(FileInfo assetFile)
        {
            using (BinaryReader reader = new BinaryReader(assetFile.OpenRead()))
            {
                Guid guid = new Guid(reader.ReadBytes(16));
                AssetType assetType = (AssetType)reader.ReadInt32();

                Asset currentAsset = null;
                switch (assetType)
                {
                    case AssetType.Unkown:
                        break;
                    case AssetType.Material:
                        currentAsset = new Material(assetFile, guid);
                        break;
                }

                currentAsset.Read(reader);
            }
        }

        private void WriteFuturaAsset(Asset asset)
        {
            using (BinaryWriter writer = new BinaryWriter(asset.Path.Open(FileMode.OpenOrCreate)))
            {
                writer.Write(asset.Identifier.ToByteArray());
                writer.Write((int)asset.AssetType);
                asset.Write(writer);
            }
        }

        public T GetAsset<T>(Guid guid) where T : Asset
        {
            if (loadedAssets.ContainsKey(guid))
            {
                return loadedAssets[guid] as T;
            }
            else return null;
        }

        public void CreateMaterial()
        {
            FileInfo[] files = rootDir.GetFiles();
            string fileName = "Material" + AssetFileExtension;

            


            FileInfo filePath = new FileInfo(rootDir.FullName + fileName);



            Material material = new Material(filePath, Guid.NewGuid());
            loadedAssets.Add(material.Identifier, material);
            WriteFuturaAsset(material);
        }


    }
}
