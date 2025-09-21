﻿namespace Cofoundry.Domain.Tests.Integration.Pages.Queries;

[Collection(nameof(IntegrationTestFixtureCollection))]
public class GetAllPageRoutesQueryHandlerTests
{
    const string UNIQUE_PREFIX = "GAllPageRoutesQHT ";

    private readonly IntegrationTestApplicationFactory _appFactory;

    public GetAllPageRoutesQueryHandlerTests(
        IntegrationTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task ReturnsAllRoutes()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(ReturnsAllRoutes);

        using var app = _appFactory.Create();
        var testPages = new Dictionary<int, string>();

        for (var i = 1; i < 3; i++)
        {
            var parentDirectoryId = await app.TestData.PageDirectories().AddAsync(uniqueData + i);

            for (var j = 1; j < 4; j++)
            {
                var pageTitle = uniqueData + j;
                var pageId = await app.TestData.Pages().AddAsync(pageTitle, parentDirectoryId);
                testPages.Add(pageId, pageTitle);
            }
        }

        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var results = await contentRepository
            .Pages()
            .GetAll()
            .AsRoutes()
            .ExecuteAsync();

        using (new AssertionScope())
        {
            foreach (var testPage in testPages)
            {
                results.Should().Contain(p => p.PageId == testPage.Key && p.Title == testPage.Value);
            }
        }
    }
}
