using Cofoundry.Core.EmbeddedResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;

namespace Cofoundry.Web
{
    /// <summary>
    /// IResourceFile abstraction over a website VirtualFile, which can be a file
    /// on disk or an embedded resource
    /// </summary>
    public class VirtualResourceFile : IResourceFile
    {
        private readonly VirtualFile _virtualFile;

        public VirtualResourceFile(VirtualFile virtualFile)
        {
            _virtualFile = virtualFile;
        }

        public string Name
        {
            get
            {
                return _virtualFile.Name;
            }
        }

        public string VirtualPath
        {
            get
            {
                return _virtualFile.VirtualPath;
            }
        }

        public Stream Open()
        {
            return _virtualFile.Open();
        }
    }
}