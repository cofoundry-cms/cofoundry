using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.ResourceFiles.Internal
{
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
                .Select(a => embeddedFileProviderFactory.Create(a));
        }

        public IFileProvider Create()
        {
            return new CompositeFileProvider(_providers);
        }
    }
}
