using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Tests.Users.Services;

public class InMemoryUserSessionServiceTests : UserSessionServiceTests
{
    protected override IUserSessionService CreateService(IUserAreaDefinitionRepository repository)
    {
        return new InMemoryUserSessionService(repository, new UserContextCache());
    }
}
