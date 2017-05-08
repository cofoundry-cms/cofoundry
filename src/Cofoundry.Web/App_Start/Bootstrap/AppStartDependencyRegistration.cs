using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Web
{
    public class AppStartDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterAll<IStartupTask>()
                .RegisterAll<IMvcJsonOptionsConfiguration>()
                .RegisterAll<IMvcOptionsConfiguration>()
                .RegisterAll<IRazorViewEngineOptionsConfiguration>()
                .RegisterType<IMvcBuilderConfiguration, CofoundryMvcBuilderConfiguration>()
            ;
        }
    }
}
