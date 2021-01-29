using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Registration
{
    public class MvcDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<IRazorViewRenderer, RazorViewRenderer>()
                .Register<IStaticFilePathFormatter, StaticFilePathFormatter>()
                .Register<IStaticFileViewHelper, StaticFileViewHelper>()
                .Register<ICofoundryHelper, CofoundryPageHelper>()
                .RegisterGeneric(typeof(ICofoundryHelper<>), typeof(CofoundryPageHelper<>))
                .RegisterGeneric(typeof(ICofoundryTemplateHelper<>), typeof(CofoundryTemplatePageHelper<>))
                .RegisterGeneric(typeof(ICofoundryBlockTypeHelper<>), typeof(CofoundryPageBlockTypeHelper<>))
                .RegisterGeneric(typeof(IPageTemplateHelper<>), typeof(PageTemplateHelper<>))
                ; 
        }
    }
}