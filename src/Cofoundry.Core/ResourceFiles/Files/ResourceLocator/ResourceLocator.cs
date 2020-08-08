using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.ResourceFiles.Internal
{
    public class ResourceLocator : IResourceLocator
    {
        private readonly IFileProvider _fileProvider;

        public ResourceLocator(
            IResourceFileProviderFactory resourceFileProviderFactory
            )
        {
            _fileProvider = resourceFileProviderFactory.Create();
        }

        public bool DirectoryExists(string virtualDir)
        {
            var directory = _fileProvider.GetDirectoryContents(virtualDir);
            return directory.Exists;
        }

        public bool FileExists(string virtualPath)
        {
            var file = _fileProvider.GetFileInfo(virtualPath);
            return file.Exists;
        }

        public IDirectoryContents GetDirectory(string virtualDir)
        {
            return _fileProvider.GetDirectoryContents(virtualDir);
        }

        public IFileInfo GetFile(string virtualPath)
        {
            var file = _fileProvider.GetFileInfo(virtualPath);
            return file;
        }
    }
}
