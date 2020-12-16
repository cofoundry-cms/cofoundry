using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration
{
    public class PageBlockTypeDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<IPageBlockTypeDataModelTypeFactory, PageBlockTypeDataModelTypeFactory>()
                .Register<IPageBlockTypeCache, PageBlockTypeCache>()
                .Register<IPageBlockTypeRepository, PageBlockTypeRepository>()
                .Register<IPageBlockTypeViewFileLocator, PageBlockTypeViewFileLocator>()
                .Register<IPageBlockTypeFileNameFormatter, PageBlockTypeFileNameFormatter>()
                .Register<IPageBlockTypeSummaryMapper, PageBlockTypeSummaryMapper>()
                .Register<IPageBlockTypeDetailsMapper, PageBlockTypeDetailsMapper>()
                .RegisterAll<IPageBlockTypeDataModel>()
                .RegisterAllGenericImplementations(typeof(IPageBlockTypeDisplayModelMapper<>))
                .RegisterAll<IPageBlockTypeViewLocationRegistration>()
                ; 
        }
    }
}
