namespace Cofoundry.Domain.Tests.Integration.UserAreas.Queries;

[Collection(nameof(DbDependentFixtureCollection))]
public class GetUserAreaMicroSummaryByCodeQueryHandlerTests
{
    private readonly DbDependentTestApplicationFactory _appFactory;

    public GetUserAreaMicroSummaryByCodeQueryHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task WhenNotExists_ReturnsNull()
    {
        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var area = await contentRepository
            .UserAreas()
            .GetByCode("ZZZ")
            .AsMicroSummary()
            .ExecuteAsync();

        area.Should().BeNull();
    }

    [Fact]
    public async Task ReturnsAndMaps()
    {
        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var area1 = app.SeededEntities.TestUserArea1.Definition;

        var testArea1 = await contentRepository
            .UserAreas()
            .GetByCode(area1.UserAreaCode)
            .AsMicroSummary()
            .ExecuteAsync();

        using (new AssertionScope())
        {
            testArea1.Should().NotBeNull();
            testArea1.Name.Should().Be(testArea1.Name);
        }
    }
}
