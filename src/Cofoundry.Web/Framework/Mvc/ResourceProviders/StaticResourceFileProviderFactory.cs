using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Cofoundry.Core;
using Cofoundry.Core.ResourceFiles;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Web
{
    /// <summary>
    /// Used to build the single-instance StaticResourceFileProvider by creating a child
    /// dependency scope to resolve various registration elements safely.
    /// </summary>
    public class StaticResourceFileProviderFactory : IInjectionFactory<StaticResourceFileProvider>
    {
        private readonly IServiceProvider _serviceProvider;

        public StaticResourceFileProviderFactory(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        public StaticResourceFileProvider Create()
        {
            // Create instance in a child scope so we don't have to dictate single-instance scope to components
            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                // resolve depedencies
                var hostingEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
                var embeddedResourceRouteRegistrations = serviceProvider.GetRequiredService<IEnumerable<IEmbeddedResourceRouteRegistration>>();
                var embeddedFileProviderFactory = serviceProvider.GetRequiredService<IEmbeddedFileProviderFactory>();

                // build file provider list
                var allFileProviders = GetAllFileProviders(hostingEnvironment, embeddedResourceRouteRegistrations, embeddedFileProviderFactory);
                IFileProvider compositeFileProvider;

                if (allFileProviders.Count == 1)
                {
                    compositeFileProvider = allFileProviders.First();
                }
                else
                {

                    compositeFileProvider = new CompositeFileProvider(allFileProviders);
                }

                return new StaticResourceFileProvider(compositeFileProvider, allFileProviders);
            }
        }

        private List<IFileProvider> GetAllFileProviders(
            IWebHostEnvironment hostingEnvironment,
            IEnumerable<IEmbeddedResourceRouteRegistration> embeddedResourceRouteRegistrations,
            IEmbeddedFileProviderFactory embeddedFileProviderFactory
            )
        {
            var allFileProviders = new List<IFileProvider>();
            allFileProviders.Add(hostingEnvironment.WebRootFileProvider);

            var assemblyRoutes = EnumerableHelper
                .Enumerate(embeddedResourceRouteRegistrations)
                .SelectMany(r => r.GetEmbeddedResourcePaths())
                .GroupBy(r => r.Assembly)
                .ToList();

            foreach (var assemblyRoute in assemblyRoutes)
            {
                var fileProvider = embeddedFileProviderFactory.Create(assemblyRoute.Key);
                foreach (var route in assemblyRoute)
                {
                    var filteredFileProvider = new FilteredEmbeddedFileProvider(
                        fileProvider, 
                        route.Path, 
                        route.RewriteFrom,
                        hostingEnvironment.ContentRootFileProvider
                        );
                    allFileProviders.Add(filteredFileProvider);
                }
            }
            
            return allFileProviders;
        }
    }
}