using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Web.Registration
{
    public class AppStartDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterAll<IStartupServiceConfigurationTask>()
                .RegisterAll<IStartupConfigurationTask>()
                .RegisterAll<IMvcJsonOptionsConfiguration>()
                .RegisterAll<IMvcOptionsConfiguration>()
                .RegisterAll<IMvcRazorRuntimeCompilationOptionsConfiguration>()
                .Register<IStaticFileOptionsConfiguration, DefaultStaticFileOptionsConfiguration>()
                .Register<IAuthConfiguration, DefaultAuthConfiguration>()
                .RegisterSingleton<AutoUpdateState>()
            ;
        }
    }
}
