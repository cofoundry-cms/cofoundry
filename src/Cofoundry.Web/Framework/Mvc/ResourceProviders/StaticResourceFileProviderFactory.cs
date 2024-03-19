﻿using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.ResourceFiles;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Cofoundry.Web;

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
        using var scope = _serviceProvider.CreateScope();

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

        return new StaticResourceFileProvider(compositeFileProvider);
    }

    private static List<IFileProvider> GetAllFileProviders(
        IWebHostEnvironment hostingEnvironment,
        IEnumerable<IEmbeddedResourceRouteRegistration> embeddedResourceRouteRegistrations,
        IEmbeddedFileProviderFactory embeddedFileProviderFactory
        )
    {
        var allFileProviders = new List<IFileProvider>
        {
            hostingEnvironment.WebRootFileProvider
        };

        var assemblyRoutes = EnumerableHelper
            .Enumerate(embeddedResourceRouteRegistrations)
            .SelectMany(r => r.GetEmbeddedResourcePaths())
            .GroupBy(r => r.Assembly)
            .ToArray();

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
