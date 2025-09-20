using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.BasicTestSite.Cofoundry.Registration;

public class ExampleDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        var overrideOptions = RegistrationOptions.Override();

        container
            .Register<IPageViewModelFactory, ExamplePageViewModelFactory>(overrideOptions)
            .Register<IPageViewModelBuilder, ExamplePageViewModelBuilder>(overrideOptions)
            ;
    }
}
