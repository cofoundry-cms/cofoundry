using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class PageDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<IPagePathHelper, PagePathHelper>()
            .Register<IPageVersionBlockModelMapper, PageVersionBlockModelMapper>()
            .Register<IPageCache, PageCache>()
            .Register<IEntityVersionPageBlockMapper, EntityVersionPageBlockMapper>()
            .Register<IPageBlockCommandHelper, PageBlockCommandHelper>()
            .RegisterAll<ICustomEntityDataModel>()
            .Register<IPageRouteLibrary, PageRouteLibrary>()
            .Register<IPageRenderDetailsMapper, PageRenderDetailsMapper>()
#pragma warning disable CS0618 // Type or member is obsolete
            .Register<IPageGroupSummaryMapper, PageGroupSummaryMapper>()
#pragma warning restore CS0618 // Type or member is obsolete
            .Register<IPageSummaryMapper, PageSummaryMapper>()
            .Register<IOpenGraphDataMapper, OpenGraphDataMapper>()
            .Register<IPageVersionSummaryMapper, PageVersionSummaryMapper>()
            .Register<IPageRenderSummaryMapper, PageRenderSummaryMapper>()
            .Register<IEntityAccessRuleSetMapper, EntityAccessRuleSetMapper>()
             ;
    }
}
