using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration
{
    public class UsersDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterAll<IUserAreaDefinition>()
                .RegisterSingleton<IUserAreaDefinitionRepository, UserAreaDefinitionRepository>()
                .Register<UserContextMapper>()
                .Register<IPasswordUpdateCommandHelper, PasswordUpdateCommandHelper>()
                .Register<IUserContextService, UserContextService>()
                .Register<IUserSessionService, InMemoryUserSessionService>(RegistrationOptions.Scoped())
                .Register<IUserSignInService, UserSignInService>()
                .Register<IUserMicroSummaryMapper, UserMicroSummaryMapper>()
                .Register<IUserSummaryMapper, UserSummaryMapper>()
                .Register<IUserDetailsMapper, UserDetailsMapper>()
                .Register<IAuthorizedTaskTokenUrlHelper, AuthorizedTaskTokenUrlHelper>()
                .Register<IUserContextCache, UserContextCache>(RegistrationOptions.Scoped())
                .Register<IUserDataFormatter, UserDataFormatter>()
                .Register<IUserUpdateCommandHelper, UserUpdateCommandHelper>()
                .RegisterAllGenericImplementations(typeof(IEmailAddressNormalizer<>))
                .Register<IEmailAddressValidator, EmailAddressValidator>()
                .Register<IUsernameValidator, UsernameValidator>()
                .Register<ISecurityStampGenerator, SecurityStampGenerator>()
                .Register<IUserSecurityStampUpdateHelper, UserSecurityStampUpdateHelper>()
                ;
        }
    }
}