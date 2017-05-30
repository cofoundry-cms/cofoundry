using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using System.Reflection;

namespace Cofoundry.Core.ResourceFiles
{
    /// <summary>
    /// A wrapper for EmbeddedFileProvider that makes it behave a little more
    /// like the physical file provider e.g. GetDirectoryContents now works
    /// for subdirectories, returning directories and files. Due to the limitations
    /// of embedded resources there are issues with translating file paths with
    /// special characters so these should be avoided. The general rule is avoid 
    /// anything that wouldn't be permitted in a class name. E.g. "jQuery.UI-2.1.1" 
    /// could be renamed "jQuery_UI_2_1_1".
    /// </summary>
    public class CofoundryEmbeddedFileProvider : IFileProvider
    {
        private readonly EmbeddedFileProvider _embeddedResourceProvider;

        public CofoundryEmbeddedFileProvider(Assembly assembly)
        {
            _embeddedResourceProvider = new EmbeddedFileProvider(assembly);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (subpath == null)
            {
                return new NotFoundDirectoryContents();
            }

            var resourcePath = subpath.Replace("-", "_")
                .Replace("~/", "")
                .Replace("/", ".")
                .Replace("\\", ".")
                .Trim(new char[] { '.' });

            var allFiles = _embeddedResourceProvider
                .GetDirectoryContents(string.Empty)
                .Where(f => f.Name.StartsWith(resourcePath, StringComparison.OrdinalIgnoreCase))
                ;

            if (!allFiles.Any()) return new NotFoundDirectoryContents();

            var directoryContents = new EmbeddedDirectoryContents(resourcePath, allFiles);

            return directoryContents;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var file =  _embeddedResourceProvider.GetFileInfo(subpath);
            return file;
        }

        public IChangeToken Watch(string filter)
        {
            return _embeddedResourceProvider.Watch(filter);
        }
    }
}
