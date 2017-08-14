using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain.Bootstrap
{
    public class PageBlockTypeDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<IPageBlockTypeDataModelTypeFactory, PageBlockTypeDataModelTypeFactory>()
                .RegisterType<IPageBlockTypeCache, PageBlockTypeCache>()
                .RegisterType<IPageBlockTypeRepository, PageBlockTypeRepository>()
                .RegisterType<IPageBlockTypeViewFileLocator, PageBlockTypeViewFileLocator>()
                .RegisterType<IPageBlockTypeFileNameFormatter, PageBlockTypeFileNameFormatter>()
                .RegisterAll<IPageBlockTypeDataModel>()
                .RegisterAllGenericImplementations(typeof(IPageBlockTypeDisplayModelMapper<>))
                .RegisterAll<IPageBlockTypeViewLocationRegistration>()
                ; 
        }
    }
}
