using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Cofoundry.Core;
using Cofoundry.Core.ResourceFiles;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;

namespace Cofoundry.Web
{
    /// <summary>
    /// A wrapper file provider that gives access to a single
    /// file provider that can access all registered static resource 
    /// locations.
    /// </summary>
    public class StaticResourceFileProvider : IStaticResourceFileProvider
    {
        private readonly IFileProvider _compositeFileProvider;

        private readonly IReadOnlyList<IFileProvider> _allFileProviders;

        public StaticResourceFileProvider(
            IFileProvider compositeFileProvider,
            IReadOnlyList<IFileProvider> allFileProviders
            )
        {
            _compositeFileProvider = compositeFileProvider;
            _allFileProviders = allFileProviders;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return _compositeFileProvider.GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var fileInfo = _compositeFileProvider.GetFileInfo(subpath);
            return fileInfo;
        }

        public IChangeToken Watch(string filter)
        {
            return _compositeFileProvider.Watch(filter);
        }
    }
}