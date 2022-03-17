using Microsoft.Extensions.FileProviders;

namespace Cofoundry.Core.ResourceFiles;

public interface IResourceFileProviderRegistration
{
    IEnumerable<IFileProvider> GetFileProviders();
}
