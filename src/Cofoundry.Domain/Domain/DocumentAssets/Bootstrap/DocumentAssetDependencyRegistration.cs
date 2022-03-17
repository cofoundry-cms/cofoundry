using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class DocumentAssetDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<DocumentAssetCommandHelper>()
            .Register<IDocumentAssetRouteLibrary, DocumentAssetRouteLibrary>()
            .Register<IDocumentAssetSummaryMapper, DocumentAssetSummaryMapper>()
            .Register<IDocumentAssetDetailsMapper, DocumentAssetDetailsMapper>()
            .Register<IDocumentAssetRenderDetailsMapper, DocumentAssetRenderDetailsMapper>()
            ;
    }
}
