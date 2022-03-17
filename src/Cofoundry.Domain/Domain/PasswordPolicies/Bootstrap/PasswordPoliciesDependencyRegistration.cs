using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration;

public class PasswordPoliciesDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<UserAuthenticationHelper, UserAuthenticationHelper>()
            .Register<UserCommandPermissionsHelper, UserCommandPermissionsHelper>()
            .Register<IPasswordPolicyService, PasswordPolicyService>()
            .Register<IPasswordPolicyFactory, PasswordPolicyFactory>()
            .Register<IDefaultPasswordPolicyConfiguration, DefaultPasswordPolicyConfiguration>()
            .RegisterAllGenericImplementations(typeof(IPasswordPolicyConfiguration<>))
            .RegisterAll<INewPasswordValidatorBase>()
            ;
    }
}
