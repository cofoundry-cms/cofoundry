namespace Cofoundry.Web.Tests.Integration.Framework.Auth.Attributes;

[Collection(nameof(TestWebApplicationFactory))]
public class AuthorizeRoleAttributeTests
{
    private readonly TestWebApplicationFactory _webApplicationFactory;

    public AuthorizeRoleAttributeTests(
        TestWebApplicationFactory webApplicationFactory
        )
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task WhenUserValid_OK()
    {
        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClient();

        await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleA.User);
        var result = await client.GetAsync(GetRoute("role"), TestContext.Current.CancellationToken);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WhenUserNotSignedIn_RedirectsToSignIn()
    {
        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClient(o => o.AllowAutoRedirect = false);

        await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.RoleA.User);
        var result = await client.GetAsync(GetRoute("role"), TestContext.Current.CancellationToken);

        SignInRedirectAssertions.AssertSignInRedirect(result, app.SeededEntities.TestUserArea2);
    }

    [Fact]
    public async Task WhenUserNotAuthorized_403()
    {
        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClient();

        await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea2.RoleB.User);
        var result = await client.GetAsync(GetRoute("role"), TestContext.Current.CancellationToken);

        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private static string GetRoute(string path)
    {
        return $"/{AuthAttributeTestController.RouteBase}/{path}";
    }
}
