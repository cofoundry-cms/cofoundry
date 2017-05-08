using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.Bootstrap
{
    public class PageModuleDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<IPageModuleDataModelTypeFactory, PageModuleDataModelTypeFactory>()
                .RegisterType<IPageModuleTypeCache, PageModuleTypeCache>()
                .RegisterType<IPageModuleTypeRepository, PageModuleTypeRepository>()
                .RegisterType<IPageModuleTypeViewFileLocator, PageModuleTypeViewFileLocator>()
                .RegisterType<IPageModuleTypeFileNameFormatter, PageModuleTypeFileNameFormatter>()
                .RegisterAll<IPageModuleDataModel>()
                .RegisterAllGenericImplementations(typeof(IPageModuleDisplayModelMapper<>))
                .RegisterAll<IPageModuleViewLocationRegistration>()
                ; 
        }
    }
}
