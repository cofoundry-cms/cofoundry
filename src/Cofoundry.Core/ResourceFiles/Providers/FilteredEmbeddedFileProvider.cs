using Conditions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
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
        private readonly EmbeddedFileProvider _assemblyProvider;
        private readonly string _restrictToPath;

        /// <summary>
        /// A file provider that wraps an EmbeddedFileProvider and restricts
        /// access to a specific path. Used in tandem with IEmbeddedResourceRouteRegistration
        /// to provide static file access to specific embedded resource paths.
        /// </summary>
        /// <param name="assemblyProvider">
        /// An EmbeddedFileProvider instance instantiated with the assembly containing the 
        /// embedded resources to serve.</param>
        /// <param name="filterToPath">The relative file path to restrict file access to e.g. '/parent/child/content'.</param>
        public FilteredEmbeddedFileProvider(
            EmbeddedFileProvider assemblyProvider,
            string filterToPath
            )
        {
            Condition.Requires(assemblyProvider).IsNotNull();
            Condition.Requires(filterToPath).IsNotNullOrEmpty();

            _restrictToPath = filterToPath.TrimStart('~');
            Condition.Requires(_restrictToPath).StartsWith("/", nameof(filterToPath) + " must start with a forward slash.");
            Condition.Requires(_restrictToPath).IsLongerThan(1, nameof(filterToPath) + " cannot be the root directory.");

            _assemblyProvider = assemblyProvider;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (subpath == null || !subpath.StartsWith(_restrictToPath))
            {
                return NotFoundDirectoryContents.Singleton;
            }

            return _assemblyProvider.GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (string.IsNullOrEmpty(subpath) || !subpath.StartsWith(_restrictToPath))
            {
                return new NotFoundFileInfo(subpath);
            }

            return _assemblyProvider.GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return _assemblyProvider.Watch(filter);
        }
    }
}
