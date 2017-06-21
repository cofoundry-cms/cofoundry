using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Web.Admin
{
    public class CofoundryAdminDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var options = new RegistrationOptions()
            {
                RegistrationOverridePriority = (int)RegistrationOverridePriority.Low,
                ReplaceExisting = true
            };

            container
                .RegisterAll<IStandardAngularModuleRegistration>()
                .RegisterType<StandardAngularModuleRegistrationBootstrapper>()
                .RegisterType<ISetupPageActionFactory, SetupPageActionFactory>(options)
                .RegisterInstance<IAdminRouteLibrary, AdminRouteLibrary>()
                .RegisterType<IAngularBootstrapper, AngularBootstrapper>()
                .RegisterType<IStaticResourceReferenceRenderer, StaticResourceReferenceRenderer>()
                ;
        }
    }
}