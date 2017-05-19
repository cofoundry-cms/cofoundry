using Cofoundry.Core.DependencyInjection;
using Cofoundry.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.BasicTestSite.Cofoundry
{
public class ExampleDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        var overrideOptions = RegistrationOptions.Override();

        container
            .RegisterType<IPageViewModelFactory, ExamplePageViewModelFactory>(overrideOptions)
            .RegisterType<IPageViewModelBuilder, ExamplePageViewModelBuilder>(overrideOptions)
            ;
    }
}
}