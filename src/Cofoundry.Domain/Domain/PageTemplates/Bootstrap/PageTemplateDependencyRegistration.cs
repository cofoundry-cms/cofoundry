using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration
{
    public class PageTemplateDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<IPageTemplateCustomEntityTypeMapper, PageTemplateCustomEntityTypeMapper>()
                .Register<IPageTemplateMicroSummaryMapper, PageTemplateMicroSummaryMapper>()
                .Register<IPageTemplateSummaryMapper, PageTemplateSummaryMapper>()
                .Register<IPageTemplateDetailsMapper, PageTemplateDetailsMapper>()
                
                .RegisterAll<IPageTemplateViewLocationRegistration>()
                ;
        }
    }
}
