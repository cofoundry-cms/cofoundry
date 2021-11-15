using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Domain.Tests.Domain
{
    public class InMemoryUserSessionServiceTests : UserSessionServiceTests
    {
        protected override IUserSessionService CreateService(IUserAreaDefinitionRepository repository)
        {
            return new InMemoryUserSessionService(repository, new UserContextCache());
        }
    }
}
