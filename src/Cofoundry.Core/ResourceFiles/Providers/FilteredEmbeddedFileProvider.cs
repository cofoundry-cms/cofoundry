using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Core.ResourceFiles
{
    /// <summary>
    /// A file provider that wraps an EmbeddedFileProvider and restricts
    /// access to a specific path. Used in tandem with IEmbeddedResourceRouteRegistration
    /// to provide static file access to specific embedded resource paths.
    /// </summary>
    public class FilteredEmbeddedFileProvider : IFileProvider
    {
        private readonly IFileProvider _assemblyProvider;
        private readonly IFileProvider _overrideProvider;
        private readonly string _restrictToPath;
        private readonly string _rewriteFromPath = null;

        #region constructor

        /// <summary>
        /// A file provider that wraps an EmbeddedFileProvider and restricts
        /// access to a specific path. Used in tandem with IEmbeddedResourceRouteRegistration
        /// to provide static file access to specific embedded resource paths.
        /// </summary>
        /// <param name="assemblyProvider">
        /// An EmbeddedFileProvider instance instantiated with the assembly containing the 
        /// embedded resources to serve.</param>
        /// <param name="filterToPath">The relative file path to restrict file access to e.g. '/parent/child/content'.</param>
        /// <param name="rewriteFromPath">
        /// The relative file path to rewrite file requests from e.g. '/virtual/path/to/content'. The 
        /// rewriteFromPath value will be replaced by filterToPath in the underlying file request. Pass
        /// null if rewriting is not required.
        /// </param>
        /// <param name="overrideProvider">
        /// A file provider that can contain files that override the assembly provider. Typically
        /// this is a physical file provider for the website root so projects can override files embedded
        /// in Cofoundry with their own versions.
        /// </param>
        public FilteredEmbeddedFileProvider(
            IFileProvider assemblyProvider,
            string filterToPath,
            string rewriteFromPath,
            IFileProvider overrideProvider = null
            )
        {
            if (assemblyProvider == null) throw new ArgumentNullException(nameof(assemblyProvider));
            if (filterToPath == null) throw new ArgumentNullException(nameof(filterToPath));
            if (string.IsNullOrWhiteSpace(filterToPath)) throw new ArgumentEmptyException(nameof(filterToPath));

            _restrictToPath = ValidatePath(filterToPath, nameof(filterToPath));

            if (!string.IsNullOrEmpty(rewriteFromPath))
            {
                var formattedRewritePath = ValidatePath(rewriteFromPath, nameof(rewriteFromPath));

                if (!rewriteFromPath.Equals(filterToPath, StringComparison.OrdinalIgnoreCase))
                {
                    _rewriteFromPath = formattedRewritePath;
                }
            }

            _assemblyProvider = assemblyProvider;
            _overrideProvider = overrideProvider;
        }

        private static string ValidatePath(string pathToTest, string parameterName)
        {
            if (!pathToTest.StartsWith("/"))
            {
                throw new ArgumentException(parameterName + " must start with a forward slash.");
            }

            if (pathToTest.Length <= 1)
            {
                throw new ArgumentException(parameterName + " cannot be the root directory.");
            }

            var formattedRewritePath = pathToTest.TrimStart('~');

            if (!formattedRewritePath.EndsWith("/"))
            {
                formattedRewritePath += '/';
            }

            return formattedRewritePath;
        }

        #endregion

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (!StartsWithPath(subpath))
            {
                return NotFoundDirectoryContents.Singleton;
            }

            var rewrittenPath = RewritePath(subpath);

            return _assemblyProvider.GetDirectoryContents(rewrittenPath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (!StartsWithPath(subpath))
            {
                return new NotFoundFileInfo(subpath);
            }

            var rewrittenPath = RewritePath(subpath);

            if (_overrideProvider != null)
            {
                var overrideFile = _overrideProvider.GetFileInfo(rewrittenPath);
                if (overrideFile != null && overrideFile.Exists)
                {
                    return overrideFile;
                }
            }

            var fileInfo = _assemblyProvider.GetFileInfo(rewrittenPath);
            return fileInfo;
        }

        public IChangeToken Watch(string filter)
        {
            return _assemblyProvider.Watch(filter);
        }

        private string RewritePath(string path)
        {
            if (_rewriteFromPath == null) return path;

            var newPath = _restrictToPath + path.Remove(0, _rewriteFromPath.Length);

            return newPath;
        }

        private bool StartsWithPath(string subpath)
        {
            if (string.IsNullOrEmpty(subpath)) return false;

            var pathToTest = _rewriteFromPath ?? _restrictToPath;

            return subpath.StartsWith(pathToTest, StringComparison.OrdinalIgnoreCase);
        }
    }
}
