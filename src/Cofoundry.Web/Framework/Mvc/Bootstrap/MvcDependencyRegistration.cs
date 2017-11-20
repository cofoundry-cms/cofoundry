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
                .RegisterType<ICofoundryPageHelper, CofoundryPageHelper>()
                .RegisterGeneric(typeof(ICofoundryPageHelper<>), typeof(CofoundryPageHelper<>))
                .RegisterGeneric(typeof(ICofoundryTemplatePageHelper<>), typeof(CofoundryTemplatePageHelper<>))
                .RegisterGeneric(typeof(ICofoundryPageBlockTypeHelper<>), typeof(CofoundryPageBlockTypeHelper<>))
                .RegisterGeneric(typeof(IPageTemplateHelper<>), typeof(PageTemplateHelper<>))
                ; 
        }
    }
}