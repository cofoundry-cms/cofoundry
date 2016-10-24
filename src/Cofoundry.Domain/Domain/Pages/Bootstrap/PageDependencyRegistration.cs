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
                .RegisterType<IPageVersionModuleModelMapper, PageVersionModuleModelMapper>()
                .RegisterType<IModuleDataModelTypeFactory, ModuleDataModelTypeFactory>()
                .RegisterType<IPageCache, PageCache>()
                .RegisterType<IModuleCache, ModuleCache>()
                .RegisterType<IEntityVersionPageModuleMapper, EntityVersionPageModuleMapper>()
                .RegisterType<IPageMetaDataMapper, PageMetaDataMapper>()
                .RegisterType<IPageModuleCommandHelper, PageModuleCommandHelper>()
                .RegisterAll<IPageModuleDataModel>()
                .RegisterAll<ICustomEntityVersionDataModel>()
                .RegisterAllGenericImplementations(typeof(IPageModuleDisplayModelMapper<>))
                .RegisterType<IPageRepository, PageRepository>()
                .RegisterType<IPageRouteLibrary, PageRouteLibrary>()
                ; 
        }
    }
}
