using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration
{
    [Collection(nameof(DbDependentFixture))]
    public class UpdateCurrentUserPasswordCommandHandlerTests
    {
        private readonly DbDependentFixture _dbDependentFixture;

        public UpdateCurrentUserPasswordCommandHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
        }
    }
}
