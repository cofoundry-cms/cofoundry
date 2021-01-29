using Cofoundry.Core.DependencyInjection;
using Cofoundry.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.BasicTestSite.Cofoundry.Registration
{
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
}