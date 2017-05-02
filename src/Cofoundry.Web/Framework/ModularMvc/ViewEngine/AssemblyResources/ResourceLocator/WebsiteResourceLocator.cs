using Cofoundry.Core.ResourceFiles;
using Cofoundry.Web.ModularMvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Cofoundry.Web
{
    /// <summary>
    /// IResourceLocator wrapper around AssemblyResourceProvider to 
    /// return both embedded assembly files and physical files
    /// </summary>
    public class WebsiteResourceLocator : IResourceLocator
    {
        #region constructor

        private readonly AssemblyResourceProvider _assemblyResourceProvider;

        public WebsiteResourceLocator(
            AssemblyResourceProvider assemblyResourceProvider
            )
        {
            _assemblyResourceProvider = assemblyResourceProvider;
        }

        public bool FileExists(string virtualPath)
        {
            return _assemblyResourceProvider.FileExists(virtualPath);
        }

        public IResourceFile GetFile(string virtualPath)
        {
            var file = _assemblyResourceProvider.GetFile(virtualPath);

            return new VirtualResourceFile(file);
        }

        public bool DirectoryExists(string virtualDir)
        {
            return _assemblyResourceProvider.DirectoryExists(virtualDir);
        }

        public IResourceDirectory GetDirectory(string virtualDir)
        {
            var directory = _assemblyResourceProvider.GetDirectory(virtualDir);

            return new VirtualResourceDirectory(directory);
        }

        #endregion
    }
}