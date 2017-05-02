using Cofoundry.Core.ResourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;

namespace Cofoundry.Web
{
    /// <summary>
    /// IResourceDirectory abstraction over a website VirtualDirectory, which can be a directory
    /// on disk or a collection of embedded resources
    /// </summary>
    public class VirtualResourceDirectory : IResourceDirectory
    {
        private readonly VirtualDirectory _virtualDirectory;

        public VirtualResourceDirectory(VirtualDirectory virtualDirectory)
        {
            _virtualDirectory = virtualDirectory;
        }

        public IEnumerable<IResourceFile> GetFiles()
        {
            foreach (var file in _virtualDirectory.Files)
            {
                if (file is VirtualFile)
                {
                    yield return new VirtualResourceFile((VirtualFile)file);
                }
            }
        }

        public IEnumerable<IResourceDirectory> GetDirectories()
        {
            foreach (var directory in _virtualDirectory.Directories)
            {
                if (directory is VirtualDirectory)
                {
                    yield return new VirtualResourceDirectory((VirtualDirectory)directory);
                }
            }
        }

        public string VirtualPath { get { return _virtualDirectory.VirtualPath; } }
    }
}