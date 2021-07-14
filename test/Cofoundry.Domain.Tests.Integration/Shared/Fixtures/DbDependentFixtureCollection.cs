using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration
{
    [CollectionDefinition(nameof(DbDependentFixture))]
    public class DbDependentFixtureCollection : ICollectionFixture<DbDependentFixture>
    {
    }
}
