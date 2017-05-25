using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Primitives;
using Cofoundry.Core;
using Cofoundry.Core.ResourceFiles;
using Microsoft.AspNetCore.Hosting;

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

        /// <summary>
        /// Eventually we should add static resource setting here to allow each provider
        /// to have things like caching options.
        /// </summary>
        private readonly IReadOnlyList<IFileProvider> _fileProviders;

        public StaticResourceFileProvider(
            IHostingEnvironment hostingEnvironment,
            IEnumerable<IEmbeddedResourceRouteRegistration> embeddedResourceRouteRegistrations
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
                    var assembly = embeddedResourceRouteRegistrations.GetType().Assembly;
                    var fileProvider = new EmbeddedFileProvider(assembly);
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
            return _compositeFileProvider.GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return _compositeFileProvider.Watch(filter);
        }
    }
}