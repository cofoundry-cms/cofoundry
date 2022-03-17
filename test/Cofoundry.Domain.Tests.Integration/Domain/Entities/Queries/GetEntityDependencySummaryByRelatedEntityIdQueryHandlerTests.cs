namespace Cofoundry.Domain.Tests.Integration.Entities.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class GetEntityDependencySummaryByRelatedEntityIdQueryHandlerTests
{
    const string UNIQUE_PREFIX = "GEntDepSumByRelIdQHT ";

    private readonly DbDependentTestApplicationFactory _appFactory;

    public GetEntityDependencySummaryByRelatedEntityIdQueryHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task MapsBasicData()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(MapsBasicData);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        await app.TestData.UnstructuredData().AddAsync<PageDirectoryEntityDefinition>(directoryId, RelatedEntityCascadeAction.None);

        var dependencies = await contentRepository.ExecuteQueryAsync(new GetEntityDependencySummaryByRelatedEntityIdQuery(PageDirectoryEntityDefinition.DefinitionCode, directoryId));

        using (new AssertionScope())
        {
            // Generally entity mapping is down to the query referenced in IDependableEntityDefinition.CreateGetEntityMicroSummariesByIdRangeQuery
            // but we'll check the basic mapping for one test here
            dependencies.Should().NotBeNull();
            dependencies.Should().HaveCount(1);
            var dependency = dependencies.First();
            dependency.CanDelete.Should().BeFalse();
            dependency.Entity.Should().NotBeNull();
            dependency.Entity.RootEntityId.Should().Be(app.SeededEntities.CustomEntityForUnstructuredDataTests.CustomEntityId);
            dependency.Entity.RootEntityTitle.Should().Be(app.SeededEntities.CustomEntityForUnstructuredDataTests.Title);
            dependency.Entity.EntityDefinitionCode.Should().Be(app.SeededEntities.CustomEntityForUnstructuredDataTests.CustomEntityDefinitionCode);
            dependency.Entity.EntityDefinitionName.Should().Be(new TestCustomEntityDefinition().Name);
            dependency.Entity.IsPreviousVersion.Should().BeFalse();
        }
    }

    [Fact]
    public async Task MapsCanDelete()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(MapsCanDelete);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        await app.TestData.UnstructuredData().AddAsync<PageDirectoryEntityDefinition>(directoryId, RelatedEntityCascadeAction.Cascade);

        var dependencies = await contentRepository.ExecuteQueryAsync(new GetEntityDependencySummaryByRelatedEntityIdQuery(PageDirectoryEntityDefinition.DefinitionCode, directoryId));

        using (new AssertionScope())
        {
            dependencies.Should().HaveCount(1);
            var dependency = dependencies.First();
            dependency.CanDelete.Should().BeTrue();
        }
    }

    [Fact]
    public async Task DoesNotFetchDuplicates()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(DoesNotFetchDuplicates);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var directoryId = await app.TestData.PageDirectories().AddAsync(uniqueData);
        var pageId = await app.TestData.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);
        var pageDraftId = await app.TestData.Pages().AddDraftAsync(pageId);
        var blockId = await app.TestData.Pages().AddPlainTextBlockToTestTemplateAsync(pageDraftId);
        var customEntity1Id = await app.TestData.CustomEntities().AddAsync(uniqueData + "1");
        var customEntity2Id = await app.TestData.CustomEntities().AddAsync(uniqueData + "2");
        var relatedEntityId = await app.TestData.CustomEntities().AddAsync(uniqueData + "related");

        await app.TestData.UnstructuredData().AddAsync(
            PageEntityDefinition.DefinitionCode, pageId,
            TestCustomEntityDefinition.Code, relatedEntityId,
            RelatedEntityCascadeAction.Cascade
            );
        await app.TestData.UnstructuredData().AddAsync(
            PageVersionBlockEntityDefinition.DefinitionCode, blockId,
            TestCustomEntityDefinition.Code, relatedEntityId,
            RelatedEntityCascadeAction.None
            );
        await app.TestData.UnstructuredData().AddAsync(
            TestCustomEntityDefinition.Code, customEntity1Id,
            TestCustomEntityDefinition.Code, relatedEntityId,
            RelatedEntityCascadeAction.None
            );

        await app.TestData.UnstructuredData().AddAsync(
            TestCustomEntityDefinition.Code, customEntity2Id,
            TestCustomEntityDefinition.Code, relatedEntityId,
            RelatedEntityCascadeAction.Cascade
            );

        var dependencies = await contentRepository.ExecuteQueryAsync(new GetEntityDependencySummaryByRelatedEntityIdQuery(TestCustomEntityDefinition.Code, relatedEntityId));

        using (new AssertionScope())
        {
            dependencies.Should().HaveCount(3);
            var pageDependency = dependencies.SingleOrDefault(e => e.Entity.RootEntityId == pageId && e.Entity.EntityDefinitionCode == PageEntityDefinition.DefinitionCode);
            pageDependency.Should().NotBeNull();
            pageDependency.CanDelete.Should().BeFalse();
            dependencies.Should().ContainSingle(e => e.Entity.RootEntityId == customEntity1Id && e.Entity.EntityDefinitionCode == TestCustomEntityDefinition.Code);
            dependencies.Should().ContainSingle(e => e.Entity.RootEntityId == customEntity2Id && e.Entity.EntityDefinitionCode == TestCustomEntityDefinition.Code);
        }
    }
}
