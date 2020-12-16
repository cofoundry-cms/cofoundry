using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration
{
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
                .Register<IPageRepository, PageRepository>()
                .Register<IPageRouteLibrary, PageRouteLibrary>()
                .Register<IPageRenderDetailsMapper, PageRenderDetailsMapper>()
                .Register<IPageGroupSummaryMapper, PageGroupSummaryMapper>()
                .Register<IPageSummaryMapper, PageSummaryMapper>()
                .Register<IOpenGraphDataMapper, OpenGraphDataMapper>()
                .Register<IPageVersionSummaryMapper, PageVersionSummaryMapper>()
                .Register<IPageRenderSummaryMapper, PageRenderSummaryMapper>()
                 ; 
        }
    }
}
