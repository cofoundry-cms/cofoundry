namespace Cofoundry.Web.Tests.Integration.Framework.Auth.Attributes;

[Collection(nameof(TestWebApplicationFactory))]
public class AuthorizePermissionAttributeTests
{
    private readonly TestWebApplicationFactory _webApplicationFactory;

    public AuthorizePermissionAttributeTests(
        TestWebApplicationFactory webApplicationFactory
        )
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task PermissionOnAnonymousRole_WhenNotSignedIn_OK()
    {
        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClient();

        var result = await client.GetAsync(GetRoute("permission-on-anonymous-role"));

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Permission_WhenUserValid_OK()
    {
        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClient();

        await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.RoleA.User);
        var result = await client.GetAsync(GetRoute("permission"));

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Permission_WhenUserNotAuthorized_403()
    {
        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClient();

        await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.RoleB.User);
        var result = await client.GetAsync(GetRoute("permission"));

        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Permission_WhenNotSignedIn_RedirectsToDefaultScheme()
    {
        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClient(o => o.AllowAutoRedirect = false);

        var result = await client.GetAsync(GetRoute("permission"));

        SignInRedirectAssertions.AssertSignInRedirect(result, app.SeededEntities.TestUserArea1);
    }

    [Fact]
    public async Task CustomEntityPermission_WhenUserValid_OK()
    {
        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClient();

        await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.RoleA.User);
        var result = await client.GetAsync(GetRoute("custom-entity-permission"));

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CustomEntityPermission_WhenUserNotAuthorized_403()
    {
        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClient();

        await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.RoleB.User);
        var result = await client.GetAsync(GetRoute("custom-entity-permission"));

        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PermissionForNonDefaultUserArea_WhenUserValid_OK()
    {
        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClient();

        await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleA.User);
        var result = await client.GetAsync(GetRoute("permission-non-default-user-area"));

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PermissionForNonDefaultUserArea_WhenUserNotAuthorized_403()
    {
        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClient();

        await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleB.User);
        var result = await client.GetAsync(GetRoute("permission-non-default-user-area"));

        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private static string GetRoute(string path)
    {
        return $"/{AuthAttributeTestController.RouteBase}/{path}";
    }
}
