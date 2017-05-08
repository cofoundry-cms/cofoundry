using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.ResourceFiles;

namespace Cofoundry.Web.ModularMvc
{
    public class ModularMvcDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterAll<IBundleRegistration>()
                .RegisterAll<IRouteRegistration>()
                .RegisterType<IRouteInitializer, RouteInitializer>()
                .RegisterType<IBundleInitializer, BundleInitializer>()
                .RegisterType<IResourceLocator, WebsiteResourceLocator>(RegistrationOptions.Override(RegistrationOverridePriority.Low))
                .RegisterType<IEmptyActionContextFactory, EmptyActionContextFactory>()
                ;
        }
    }
}
