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
                ;
        }
    }
}