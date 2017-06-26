using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using System.Reflection;
using System.IO;

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
        private readonly EmbeddedFileProvider _embeddedFileProvider;
        private readonly PhysicalFileProvider _physicalFileProvider = null;

        public CofoundryEmbeddedFileProvider(
            Assembly assembly,
            DebugSettings embeddedResourcesSetting,
            IPathResolver pathResolver
            )
        {
            _embeddedFileProvider = new EmbeddedFileProvider(assembly);

            if (embeddedResourcesSetting.BypassEmbeddedContent)
            {
                var basePath = embeddedResourcesSetting.EmbeddedContentPhysicalPathRootOverride;

                if (string.IsNullOrEmpty(basePath))
                {
                    basePath = pathResolver.MapPath("/");
                }

                var path = Path.Combine(basePath, assembly.GetName().Name);

                _physicalFileProvider = new PhysicalFileProvider(path);
            }
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

            var allFiles = _embeddedFileProvider
                .GetDirectoryContents(string.Empty)
                .Where(f => f.Name.StartsWith(resourcePath, StringComparison.OrdinalIgnoreCase))
                ;

            if (!allFiles.Any()) return new NotFoundDirectoryContents();

            var directoryContents = new EmbeddedDirectoryContents(resourcePath, allFiles);

            return directoryContents;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            IFileInfo file = null;

            if (_physicalFileProvider != null)
            {
                file = _physicalFileProvider.GetFileInfo(subpath);
            }

            if (file == null || !file.Exists)
            {
                file = _embeddedFileProvider.GetFileInfo(subpath);

                if (!file.Exists)
                {
                    // To support some file paths we need to fix it by removing dashes in the directory
                    // which aren't translated well by the underlying provider.
                    var fileName = Path.GetFileName(subpath);
                    var directory = subpath.Remove(subpath.Length - fileName.Length);
                    var fixedName = directory.Replace("-", "_") + fileName;
                    file = _embeddedFileProvider.GetFileInfo(fixedName);
                }
            }

            return file;
        }

        public IChangeToken Watch(string filter)
        {
            if (_physicalFileProvider != null)
            {
                return _physicalFileProvider.Watch(filter);
            }
            return _embeddedFileProvider.Watch(filter);
        }
    }
}
