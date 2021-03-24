using Futura.Engine.Rendering;
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
        public const string MetaFileExtension = ".fMeta";
        public const string AssetFileExtension = ".fAsset";
        public DirectoryInfo RootDirectory { get; private set; }

        private Dictionary<Guid, Asset> loadedAssets = new Dictionary<Guid, Asset>();
        public Dictionary<Guid, Asset>.ValueCollection LoadedAssets { get => loadedAssets.Values; }

        private List<Importer> importers = new List<Importer>();

        private Settings.AssetSettings assetSettings;

        internal void Init(DirectoryInfo rootDirectory)
        {
            this.RootDirectory = rootDirectory;
            if (!RootDirectory.Exists) RootDirectory.Create();

            // Add all available importers
            importers.Add(new Import.MeshImporter());
            importers.Add(new Import.TextureImporter());

            assetSettings = Core.Runtime.Instance.Settings.Get<Settings.AssetSettings>();
            CheckFolderForAssets(this.RootDirectory);
        }

        /// <summary>
        /// Checks the current asset directory for all assets
        /// </summary>
        /// <param name="folder"></param>
        private void CheckFolderForAssets(DirectoryInfo folder)
        {
            foreach (var dir in folder.GetDirectories()) CheckFolderForAssets(dir);

            foreach(var file in folder.GetFiles())
            {
                if (file.Extension == MetaFileExtension) continue;

                string metaFilePath = file.FullName + MetaFileExtension;
                if (File.Exists(metaFilePath)) LoadAsset(file, new FileInfo(metaFilePath), assetSettings.AutomaticCheckForFileChange);
                else if (file.Extension == AssetFileExtension) LoadAsset(file, file, false);
                else ImportAsset(file, Guid.NewGuid());

            }
        }

        private void LoadAsset(FileInfo assetFile, FileInfo metaFile, bool checkFileChange = true)
        {
            Asset asset = Load(metaFile, out byte[] metaHash, assetFile);

            if (checkFileChange)
            {
                byte[] assetHash = Helper.CacluateHash(assetFile);

                if (!assetHash.SequenceEqual(metaHash))
                {
                    Log.Debug("File hash is not equal, reimporting the asset");
                    metaFile.Delete();
                    ImportAsset(assetFile, asset.Identifier);
                    return;
                }
            }

            loadedAssets.Add(asset.Identifier, asset);
            asset.Load();
        }

        private void ImportAsset(FileInfo file, Guid guid)
        {
            foreach(Importer i in importers)
            {
                if (i.SupportedExtensions.Contains(file.Extension))
                {
                    Asset asset = i.ImportAsset(file, guid);
                    loadedAssets.Add(asset.Identifier, asset);
                    asset.Load();
                    Core.Profiler.Report("Import", file.FullName);

                    FileInfo metaFile = new FileInfo(file.FullName + MetaFileExtension);
                    Save(asset, metaFile);
                    break;
                }
            }   
        }


        /// <summary>
        /// Gets asset specifed by guid. Return null if not found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="guid"></param>
        /// <returns></returns>
        public T GetAsset<T>(Guid guid) where T : Asset
        {
            if (guid == Guid.Empty)
            {
                return null;
            }

            if (loadedAssets.ContainsKey(guid))
            {
                return loadedAssets[guid] as T;
            }
            else return null;
        }

        /// <summary>
        /// Creates a new empty material
        /// </summary>
        public void CreateMaterial()
        {
            FileInfo[] files = RootDirectory.GetFiles();
            string fileName = "Material" + AssetFileExtension;
            int index = 0;
            while(files.Where(f=> f.Name == fileName).Count() > 0)
            {
                index++;
                fileName = $"Material({index}){AssetFileExtension}";
            }
            


            FileInfo filePath = new FileInfo(RootDirectory.FullName + "\\" + fileName);



            Material material = new Material(filePath, Guid.NewGuid());
            loadedAssets.Add(material.Identifier, material);
            Save(material);
        }

        /// <summary>
        /// Saves the changes to the meta file
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="path"></param>
        public void Save(Asset asset, FileInfo path = null)
        {
            if (path == null)
            {
                if (asset.Path.Extension == AssetFileExtension)
                    path = asset.Path;
                else
                    path = new FileInfo(asset.Path.FullName + MetaFileExtension);
            }
            if(path.Extension != MetaFileExtension && path.Extension != AssetFileExtension)
            {
                Log.Error("Tried to write t a non futura file format, aborting");
                return;
            }

            using (BinaryWriter writer = new BinaryWriter(path.Open(FileMode.OpenOrCreate)))
            {
                if(asset.AssetType == AssetType.Material)
                {
                    writer.Write(new byte[Helper.HashLength]);
                }
                else
                {
                    byte[] fileHash = Helper.CacluateHash(asset.Path);
                    writer.Write(fileHash);
                }

                writer.Write(asset.Identifier.ToByteArray());
                writer.Write((int)asset.AssetType);
                asset.Write(writer);
            }
        }

        /// <summary>
        /// Loads a meta file 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="hash"></param>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        private Asset Load(FileInfo path, out byte[] hash, FileInfo assetPath = null)
        {
            if (assetPath == null) assetPath = path;

            if (path.Extension != MetaFileExtension && path.Extension != AssetFileExtension)
            {
                Log.Error("Tried to read a non futura file format, aborting");
                hash = new byte[0];
                return null;
            }


            using (BinaryReader reader = new BinaryReader(path.OpenRead()))
            {
                hash = reader.ReadBytes(Helper.HashLength);

                Guid guid = new Guid(reader.ReadBytes(16));
                AssetType assetType = (AssetType)reader.ReadInt32();

                Asset currentAsset = null;
                switch (assetType)
                {
                    case AssetType.Unkown:
                        break;
                    case AssetType.Material:
                        currentAsset = new Material(assetPath, guid);
                        break;
                    case AssetType.Mesh:
                        currentAsset = new Mesh(assetPath, guid);
                        break;
                    case AssetType.Texture2d:
                        currentAsset = new Texture2D(assetPath, guid);
                        break;
                }

                if(currentAsset == null)
                {
                    Log.Error("Could not load asset " + path.FullName);
                    return null;
                }

                currentAsset.Read(reader);
                return currentAsset;
            }
        }


    }
}
