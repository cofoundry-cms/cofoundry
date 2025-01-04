namespace Cofoundry.Web.Tests.Integration.Framework.Auth.Attributes;

[Collection(nameof(TestWebApplicationFactory))]
public class AuthorizeUserAreaAttributeTests
{
    private readonly TestWebApplicationFactory _webApplicationFactory;

    public AuthorizeUserAreaAttributeTests(
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
        var result = await client.GetAsync(GetRoute("user-area"));

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task WhenUserNotSignedIn_RedirectsToSignIn()
    {
        using var app = _webApplicationFactory.CreateApp();
        using var client = _webApplicationFactory.CreateClient(o => o.AllowAutoRedirect = false);

        await client.ImpersonateUserAsync(app.SeededEntities.TestUserArea1.RoleA.User);
        var result = await client.GetAsync(GetRoute("user-area"));

        SignInRedirectAssertions.AssertSignInRedirect(result, app.SeededEntities.TestUserArea2);
    }

    private static string GetRoute(string path)
    {
        return $"/{AuthAttributeTestController.RouteBase}/{path}";
    }
}
