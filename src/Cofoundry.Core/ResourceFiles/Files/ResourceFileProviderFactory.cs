﻿using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Cofoundry.Core.ResourceFiles.Internal;

/// <summary>
/// This factory allows us to get hold of an IFileProvider that can be
/// configured after the container has been configured.
/// </summary>
public class ResourceFileProviderFactory : IResourceFileProviderFactory
{
    private readonly IFileProvider[] _providers;

    public ResourceFileProviderFactory(
        IEnumerable<IAssemblyResourceRegistration> assemblyResourceRegistrations,
        IEnumerable<IResourceFileProviderRegistration> resourceFileProviderRegistrations,
        IEmbeddedFileProviderFactory embeddedFileProviderFactory
        )
    {
        // Give preference to physical providers local to the project over embedded providers
        _providers = resourceFileProviderRegistrations
            .SelectMany(r => r.GetFileProviders())
            .OrderByDescending(r => r is PhysicalFileProvider)
            .ThenByDescending(r => r is CompositeFileProvider)
            .ToList()
            .Union(CreateAssemblyProviders(assemblyResourceRegistrations, embeddedFileProviderFactory))
            .ToArray();
    }

    private static IEnumerable<IFileProvider> CreateAssemblyProviders(
        IEnumerable<IAssemblyResourceRegistration> assemblyResourceRegistrations,
        IEmbeddedFileProviderFactory embeddedFileProviderFactory
        )
    {
        return assemblyResourceRegistrations
            .Select(r => r.GetType().GetTypeInfo().Assembly)
            .Distinct()
            .Select(embeddedFileProviderFactory.Create);
    }

    public IFileProvider Create()
    {
        return new CompositeFileProvider(_providers);
    }
}
