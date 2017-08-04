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

        private readonly IReadOnlyList<IFileProvider> _fileProviders;

        public StaticResourceFileProvider(
            IHostingEnvironment hostingEnvironment,
            IEnumerable<IEmbeddedResourceRouteRegistration> embeddedResourceRouteRegistrations,
            IEmbeddedFileProviderFactory embeddedFileProviderFactory
            )
        {
            var allFileProviders = new List<IFileProvider>();
            allFileProviders.Add(hostingEnvironment.WebRootFileProvider);

            if (EnumerableHelper.IsNullOrEmpty(embeddedResourceRouteRegistrations))
            {
                _compositeFileProvider = hostingEnvironment.WebRootFileProvider;
            }
            else
            {
                foreach (var embeddedResourceRouteRegistration in embeddedResourceRouteRegistrations)
                {
                    var assembly = embeddedResourceRouteRegistration.GetType().GetTypeInfo().Assembly;
                    var fileProvider = embeddedFileProviderFactory.Create(assembly);
                    foreach (var route in embeddedResourceRouteRegistration.GetEmbeddedResourcePaths())
                    {
                        allFileProviders.Add(new FilteredEmbeddedFileProvider(fileProvider, route));
                    }
                }

                _compositeFileProvider = new CompositeFileProvider(allFileProviders);
            }

            _fileProviders = allFileProviders;
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