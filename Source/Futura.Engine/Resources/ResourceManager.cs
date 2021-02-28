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

        internal void Init(DirectoryInfo rootDirectory)
        {
            this.rootDir = rootDirectory;

            if (!rootDir.Exists) rootDir.Create();
        }


        private void CheckFolderForAssets(DirectoryInfo folder)
        {
            foreach (var dir in folder.GetDirectories()) CheckFolderForAssets(dir);

            foreach(var file in folder.GetFiles())
            {
                if (file.Extension == MetaFileExtension) continue;
            }
        }
    }
}
