using Cofoundry.Core.Time;
using Cofoundry.Core.Time.Mocks;

namespace Cofoundry.Web.Tests.Integration.Framework.Auth.Claims;

[Collection(nameof(DbDependentTestApplicationFactory))]
public class ClaimsPrincipalValidatorTests
{
    const string UNIQUE_PREFIX = "ClaimsPrincipalValT";

    private readonly DbDependentTestApplicationFactory _appFactory;
    private readonly TestWebApplicationFactory _webApplicationFactory;

    public ClaimsPrincipalValidatorTests(
        DbDependentTestApplicationFactory appFactory,
        TestWebApplicationFactory webApplicationFactory
        )
    {
        _appFactory = appFactory;
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task WhenNoChange_Validates()
    {
        var seedDate = DateTimeOffset.UtcNow;
        var dateTimeService = new MockDateTimeService(seedDate);

        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClientWithServices(s => s.AddSingleton<IDateTimeService>(dateTimeService));
        var user = app.SeededEntities.TestUserArea1.RoleA.User;

        await client.ImpersonateUserAsync(user);
        dateTimeService.MockDateTime = seedDate.AddMinutes(35);
        var userId = await GetCurrentlySignedInUserId(client);

        userId.Should().Be(user.UserId);
    }

    [Fact]
    public async Task WhenSecurityStampUpdated_Invalidates()
    {
        var uniqueData = "SecurityStampUpd_Inval";

        var seedDate = DateTimeOffset.UtcNow;
        var dateTimeService = new MockDateTimeService(seedDate);

        using var app = _webApplicationFactory.CreateApp();
        using var client1 = _webApplicationFactory.CreateClientWithServices(s => s.AddSingleton<IDateTimeService>(dateTimeService));
        using var client2 = _webApplicationFactory.CreateClientWithServices(s => s.AddSingleton<IDateTimeService>(dateTimeService));
        var userId = await app.TestData.Users().AddAsync(uniqueData, UNIQUE_PREFIX);

        await client1.ImpersonateUserAsync(userId);
        await client2.ImpersonateUserAsync(userId);

        var response = await client1.PutAsync($"/tests/users/password/{userId}", null);
        response.EnsureSuccessStatusCode();

        dateTimeService.MockDateTime = seedDate.AddMinutes(35);
        var client1UserId = await GetCurrentlySignedInUserId(client1);
        var client2UserId = await GetCurrentlySignedInUserId(client2);

        client1UserId.Should().Be(userId);
        client2UserId.Should().BeNull();
    }

    private async Task<int?> GetCurrentlySignedInUserId(HttpClient client)
    {
        var response = await client.GetAsync("/tests/users/current");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();

        return IntParser.ParseOrNull(result);
    }
}
