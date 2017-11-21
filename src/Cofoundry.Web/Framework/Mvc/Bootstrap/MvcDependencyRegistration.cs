using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public class MvcDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<IRazorViewRenderer, RazorViewRenderer>()
                .RegisterType<IStaticFilePathFormatter, StaticFilePathFormatter>()
                .RegisterType<IStaticFileViewHelper, StaticFileViewHelper>()
                .RegisterType<ICofoundryHelper, CofoundryPageHelper>()
                .RegisterGeneric(typeof(ICofoundryHelper<>), typeof(CofoundryPageHelper<>))
                .RegisterGeneric(typeof(ICofoundryTemplateHelper<>), typeof(CofoundryTemplatePageHelper<>))
                .RegisterGeneric(typeof(ICofoundryBlockTypeHelper<>), typeof(CofoundryPageBlockTypeHelper<>))
                .RegisterGeneric(typeof(IPageTemplateHelper<>), typeof(PageTemplateHelper<>))
                ; 
        }
    }
}