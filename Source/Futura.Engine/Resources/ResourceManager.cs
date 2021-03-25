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

        private FileSystemWatcher fileSystemWatcher;
        private Asset lastDeletedAsset = null;


        internal void Init(DirectoryInfo rootDirectory)
        {
            this.RootDirectory = rootDirectory;
            if (!RootDirectory.Exists) RootDirectory.Create();

            // Add all available importers
            importers.Add(new Import.MeshImporter());
            importers.Add(new Import.TextureImporter());

            assetSettings = Core.Runtime.Instance.Settings.Get<Settings.AssetSettings>();
            CheckFolderForAssets(this.RootDirectory);

            fileSystemWatcher = new FileSystemWatcher(RootDirectory.FullName);
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = true;

            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
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

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (!CheckFileType(e.FullPath))
            {
                // A directory is renamed
                RenameDirectory(new DirectoryInfo(e.OldFullPath), new DirectoryInfo(e.FullPath));
            }
            else
            {
                // A file is renamed
                Asset asset = GetAssetByPath(e.OldFullPath);
                asset.Path = new FileInfo(e.FullPath);

                string metaFile = e.OldFullPath + MetaFileExtension;
                File.Copy(metaFile, e.FullPath + MetaFileExtension);
                File.Delete(metaFile);

                Log.Debug($"Asset {e.OldFullPath} was renamed to {e.FullPath}");
            }
           
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (!CheckFileType(e.FullPath)) return;

            lastDeletedAsset = GetAssetByPath(e.FullPath);
            lastDeletedAsset.IsDeleted = true;
            File.Delete(e.FullPath + MetaFileExtension);
            Log.Debug($"Asset {e.FullPath} was deleted");
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (!CheckFileType(e.FullPath)) return;

            if(lastDeletedAsset != null)
            {
                string lastFileName = Path.GetFileName(lastDeletedAsset.Path.FullName);
                string newFileName = Path.GetFileName(e.FullPath);
                if(lastFileName == newFileName)
                {
                    lastDeletedAsset.Path = new FileInfo(e.FullPath);
                    lastDeletedAsset.IsDeleted = false;
                    Save(lastDeletedAsset);
                    Log.Debug($"Asset {e.FullPath} was moved");
                    return;
                }
            }

            ImportAsset(new FileInfo(e.FullPath), Guid.NewGuid());
            Log.Debug($"Asset {e.FullPath} was created");
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!CheckFileType(e.FullPath)) return;
            Asset asset = GetAssetByPath(e.FullPath);
            if(asset != null)
            {
                asset.Unload();
                ImportAsset(new FileInfo(e.FullPath), asset.Identifier);
                Log.Debug($"Asset {e.FullPath} was changed");
            }
        }

        private void RenameDirectory(DirectoryInfo oldName, DirectoryInfo newName)
        {
            foreach(Asset a in loadedAssets.Values)
            {
                if (a.Path.FullName.Contains(oldName.FullName))
                {
                    string oldFilePath = a.Path.FullName;
                    oldFilePath = oldFilePath.Remove(0, oldName.FullName.Length);
                    string newFilePath = newName.FullName + oldFilePath;
                    a.Path = new FileInfo(newFilePath);
                }
            }


        }

        private Asset GetAssetByPath(string path)
        {
            foreach (var a in loadedAssets) if (a.Value.Path.FullName == path) return a.Value;
            return null;
        }

        private bool CheckFileType(string path)
        {
            string[] split = path.Split('.');

            if(split.Length > 1)
            {
                string ext = "." + split.Last();

                foreach (Importer i in importers)
                {
                    if (i.SupportedExtensions.Contains(ext))
                    {
                        return true;
                    }
                }

                if (ext == AssetFileExtension) return true;
            }
            return false;
        }

    }
}
