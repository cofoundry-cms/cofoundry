using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class AccessRulesDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<IUpdateAccessRuleSetCommandHelper, UpdateAccessRuleSetCommandHelper>()
            .Register<IEntityAccessRuleSetDetailsMapper, EntityAccessRuleSetDetailsMapper>()
            .Register<IEntityAccessRuleSetMapper, EntityAccessRuleSetMapper>()
            ;
    }
}
