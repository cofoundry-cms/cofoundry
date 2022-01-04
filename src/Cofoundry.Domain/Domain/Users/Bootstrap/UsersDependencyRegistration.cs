using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration
{
    public class UsersDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterAll<IUserAreaDefinition>()
                .Register<IUserAreaDefinitionRepository, UserAreaDefinitionRepository>()
                .Register<IUserRepository, UserRepository>()
                .Register<UserContextMapper>()
                .Register<IPasswordUpdateCommandHelper, PasswordUpdateCommandHelper>()
                .Register<IUserContextService, UserContextService>()
                .Register<IUserSessionService, InMemoryUserSessionService>(RegistrationOptions.Scoped())
                .Register<ILoginService, LoginService>()
                .Register<IUserMicroSummaryMapper, UserMicroSummaryMapper>()
                .Register<IUserSummaryMapper, UserSummaryMapper>()
                .Register<IUserDetailsMapper, UserDetailsMapper>()
                .Register<IPasswordResetUrlHelper, PasswordResetUrlHelper>()
                .Register<IUserContextCache, UserContextCache>(RegistrationOptions.Scoped())
                .Register<IUserDataFormatter, UserDataFormatter>()
                .Register<IUserUpdateCommandHelper, UserUpdateCommandHelper>()
                .RegisterAllGenericImplementations(typeof(IEmailAddressNormalizer<>))
                ;
        }
    }
}
