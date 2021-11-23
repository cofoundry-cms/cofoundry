using Cofoundry.Domain.Data;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Entities.Queries
{
    /// <remarks>
    /// Given <see cref="GetEntityDependencySummaryByRelatedEntityIdQuery"/> delegates to this 
    /// query the tests in <see cref="GetEntityDependencySummaryByRelatedEntityIdQueryHandlerTests"/>
    /// should cover most of this.
    /// </remarks>
    [Collection(nameof(DbDependentFixtureCollection))]
    public class GetEntityDependencySummaryByRelatedEntityIdRangeQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "GEntDepSumByRelIdRngQHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public GetEntityDependencySummaryByRelatedEntityIdRangeQueryHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CanQueryMultiple()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanQueryMultiple);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

            var customEntity1Id = await app.TestData.CustomEntities().AddAsync(uniqueData + "1");
            var customEntity2Id = await app.TestData.CustomEntities().AddAsync(uniqueData + "2");

            await app.TestData.UnstructuredData().AddAsync(
                TestCustomEntityDefinition.Code, customEntity1Id,
                TestCustomEntityDefinition.Code, customEntity2Id,
                RelatedEntityCascadeAction.None
                );

            await app.TestData.UnstructuredData().AddAsync(
                TestCustomEntityDefinition.Code, customEntity2Id,
                TestCustomEntityDefinition.Code, customEntity1Id,
                RelatedEntityCascadeAction.Cascade
                );

            var dependencies = await contentRepository.ExecuteQueryAsync(new GetEntityDependencySummaryByRelatedEntityIdRangeQuery(TestCustomEntityDefinition.Code, new int[] { customEntity1Id, customEntity2Id }));

            using (new AssertionScope())
            {
                dependencies.Should().HaveCount(2);
                dependencies.Should().ContainSingle(e => e.Entity.RootEntityId == customEntity1Id && e.Entity.EntityDefinitionCode == TestCustomEntityDefinition.Code);
                dependencies.Should().ContainSingle(e => e.Entity.RootEntityId == customEntity2Id && e.Entity.EntityDefinitionCode == TestCustomEntityDefinition.Code);
            }
        }
    }
}
