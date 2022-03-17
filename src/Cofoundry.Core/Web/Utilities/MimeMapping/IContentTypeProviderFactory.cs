using Cofoundry.Core.DependencyInjection;
using Microsoft.AspNetCore.StaticFiles;

namespace Cofoundry.Core.Web.Internal;

/// <summary>
/// Factory for creating the IContentTypeProvider that gets registered 
/// with the DI container as a single instance.
/// </summary>
public interface IContentTypeProviderFactory : IInjectionFactory<IContentTypeProvider>
{
}
