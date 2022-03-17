namespace Cofoundry.Domain.Tests.Domain.CustomEntities.RoutingRules;

public class IdAndUrlSlugCustomEntityRoutingRuleTests
{
    const string PATH = "/my-example/path/";
    private readonly IdAndUrlSlugCustomEntityRoutingRule _rule;
    private readonly PageRoute _pageRoute;

    public IdAndUrlSlugCustomEntityRoutingRuleTests()
    {
        _rule = new IdAndUrlSlugCustomEntityRoutingRule();
        _pageRoute = new PageRoute()
        {
            FullUrlPath = PATH + _rule.RouteFormat,
            CustomEntityDefinitionCode = "TSTCDE",
            Locale = new ActiveLocale()
            {
                LocaleId = 8
            }
        };
    }

    [Theory]
    [InlineData("1/test-slug-123")]
    [InlineData("123456778")]
    [InlineData("021/q")]
    [InlineData("178/178")]
    public void MatchesRule_ValidId_ReturnsTrue(string urlPart)
    {
        var url = PATH + urlPart;

        var isMatch = _rule.MatchesRule(url, _pageRoute);

        isMatch.Should().BeTrue();
    }

    [Theory]
    [InlineData("0/test-slug")]
    [InlineData("not-an-int/test-slug")]
    [InlineData("q/test-slug")]
    public void MatchesRule_WhenInvalid_ReturnsFalse(string urlPart)
    {
        var url = PATH + urlPart;

        var isMatch = _rule.MatchesRule(url, _pageRoute);

        isMatch.Should().BeFalse();
    }

    [Theory]
    [InlineData("1/test-slug-123", 1)]
    [InlineData("123456778", 123456778)]
    [InlineData("021/q", 21)]
    [InlineData("178/179", 178)]
    public void ExtractRoutingQuery_WhenValid_MapsQuery(string urlPart, int expectedId)
    {
        var url = PATH + urlPart;

        var query = _rule.ExtractRoutingQuery(url, _pageRoute) as GetCustomEntityRouteByPathQuery;

        query.Should().NotBeNull();
        query.CustomEntityDefinitionCode.Should().Be(_pageRoute.CustomEntityDefinitionCode);
        query.CustomEntityId.Should().Be(expectedId);
        query.LocaleId.Should().Be(_pageRoute.Locale.LocaleId);
        query.UrlSlug.Should().BeNull();
    }

    [Fact]
    public void ExtractRoutingQuery_WhenInvalid_Throws()
    {
        var url = PATH + "0";

        var query = _rule
            .Invoking(r => r.ExtractRoutingQuery(url, _pageRoute))
            .Should()
            .Throw<ArgumentException>()
            .WithParameterName("url");
    }

    [Theory]
    [InlineData(1, "test-slug", "/my-example/path/1/test-slug")]
    [InlineData(123456778, "123", "/my-example/path/123456778/123")]
    public void MakeUrl(int customEntityId, string urlSlug, string expectedUrl)
    {
        var customEntityRoute = new CustomEntityRoute()
        {
            CustomEntityId = customEntityId,
            UrlSlug = urlSlug
        };

        var url = _rule.MakeUrl(_pageRoute, customEntityRoute);

        url.Should().Be(expectedUrl);
    }
}
