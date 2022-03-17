using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

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
