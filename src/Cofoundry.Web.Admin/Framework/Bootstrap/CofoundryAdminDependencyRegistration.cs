using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Web.Admin.Registration
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
                .Register<StandardAngularModuleRegistrationBootstrapper>()
                .Register<ISetupPageActionFactory, SetupPageActionFactory>(options)
                .RegisterSingleton<IAdminRouteLibrary, AdminRouteLibrary>()
                .Register<IAngularBootstrapper, AngularBootstrapper>()
                .Register<IStaticResourceReferenceRenderer, StaticResourceReferenceRenderer>()
                .Register<IDashboardContentProvider, DashboardContentProvider>()
                ;
        }
    }
}