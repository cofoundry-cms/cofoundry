using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.Bootstrap
{
    public class PageDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<IPagePathHelper, PagePathHelper>()
                .RegisterType<IPageVersionBlockModelMapper, PageVersionBlockModelMapper>()
                .RegisterType<IPageCache, PageCache>()
                .RegisterType<IEntityVersionPageBlockMapper, EntityVersionPageBlockMapper>()
                .RegisterType<IPageBlockCommandHelper, PageBlockCommandHelper>()
                .RegisterAll<ICustomEntityDataModel>()
                .RegisterType<IPageRepository, PageRepository>()
                .RegisterType<IPageRouteLibrary, PageRouteLibrary>()
                .RegisterType<IPageMapper, PageMapper>()
                 ; 
        }
    }
}
