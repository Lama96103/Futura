using System;
using System.Collections.Generic;
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

        private List<Importer> importers = new List<Importer>();



        internal void Init(DirectoryInfo rootDirectory)
        {
            this.rootDir = rootDirectory;

            // Add all available importers
            importers.Add(new Import.MeshImporter());

            if (!rootDir.Exists) rootDir.Create();
        }


        private void CheckFolderForAssets(DirectoryInfo folder)
        {
            foreach (var dir in folder.GetDirectories()) CheckFolderForAssets(dir);

            foreach(var file in folder.GetFiles())
            {
                if (file.Extension == MetaFileExtension) continue;

                if (File.Exists(Path.Combine(file.FullName, MetaFileExtension))) LoadAsset(file);
                else ImportAsset(file);
            }
        }

        private void LoadAsset(FileInfo file)
        {
            throw new NotImplementedException();
        }

        private void ImportAsset(FileInfo file)
        {
            foreach(Importer i in importers)
            {
                if (i.SupportedExtensions.Contains(file.Extension))
                {
                    Asset asset = i.ImportAsset(file);
                    loadedAssets.Add(asset.Identifier, asset);
                    break;
                }
            }   
        }
    }
}
