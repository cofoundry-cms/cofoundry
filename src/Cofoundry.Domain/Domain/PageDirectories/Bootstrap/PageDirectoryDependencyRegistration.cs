using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration
{
    public class PageDirectoryDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<IPageDirectoryCache, PageDirectoryCache>()
                .Register<IPageDirectoryRouteMapper, PageDirectoryRouteMapper>()
                .Register<IPageDirectoryTreeMapper, PageDirectoryTreeMapper>()
                .Register<IPageDirectoryMicroSummaryMapper, PageDirectoryMicroSummaryMapper>()
                ;
        }
    }
}
