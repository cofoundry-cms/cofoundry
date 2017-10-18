using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.Bootstrap
{
    public class DocumentAssetDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<DocumentAssetCommandHelper>()
                .RegisterType<IDocumentAssetRouteLibrary, DocumentAssetRouteLibrary>()
                .RegisterType<IDocumentAssetRepository, DocumentAssetRepository>()
                .RegisterType<IDocumentAssetSummaryMapper, DocumentAssetSummaryMapper>()
                .RegisterType<IDocumentAssetDetailsMapper, DocumentAssetDetailsMapper>()
                .RegisterType<IDocumentAssetRenderDetailsMapper, DocumentAssetRenderDetailsMapper>()
                ;
        }
    }
}
