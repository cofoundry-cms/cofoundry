using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Domain.Tests.Domain.CustomEntities.RoutingRules
{
    public class UrlSlugCustomEntityRoutingRuleTests
    {
        const string PATH = "/my-example/path/";
        private readonly UrlSlugCustomEntityRoutingRule _rule;
        private readonly PageRoute _pageRoute;

        public UrlSlugCustomEntityRoutingRuleTests()
        {
            _rule = new UrlSlugCustomEntityRoutingRule();
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
        [InlineData("test-slug-123")]
        [InlineData("123456778")]
        public void MatchesRule_ValidId_ReturnsTrue(string urlPart)
        {
            var url = PATH + urlPart;

            var isMatch = _rule.MatchesRule(url, _pageRoute);

            isMatch.Should().BeTrue();
        }

        [Theory]
        [InlineData("!*$")]
        [InlineData("test-slug/test-slug")]
        [InlineData("123/test")]
        [InlineData("o*/test-slug")]
        public void MatchesRule_WhenInvalid_ReturnsFalse(string urlPart)
        {
            var url = PATH + urlPart;

            var isMatch = _rule.MatchesRule(url, _pageRoute);

            isMatch.Should().BeFalse();
        }

        [Theory]
        [InlineData("test-slug-123", "test-slug-123")]
        [InlineData("123456778", "123456778")]
        public void ExtractRoutingQuery_WhenValid_MapsQuery(string urlPart, string expectedUrlSlug)
        {
            var url = PATH + urlPart;

            var query = _rule.ExtractRoutingQuery(url, _pageRoute) as GetCustomEntityRouteByPathQuery;

            query.Should().NotBeNull();
            query.CustomEntityDefinitionCode.Should().Be(_pageRoute.CustomEntityDefinitionCode);
            query.UrlSlug.Should().Be(expectedUrlSlug);
            query.LocaleId.Should().Be(_pageRoute.Locale.LocaleId);
            query.CustomEntityId.Should().BeNull();
        }

        [Fact]
        public void ExtractRoutingQuery_WhenInvalid_Throws()
        {
            var url = PATH + "*";

            var query = _rule
                .Invoking(r => r.ExtractRoutingQuery(url, _pageRoute))
                .Should()
                .Throw<ArgumentException>()
                .WithParameterName("url");
        }

        [Theory]
        [InlineData("test-slug", "/my-example/path/test-slug")]
        [InlineData("123", "/my-example/path/123")]
        public void MakeUrl(string urlSlug, string expectedUrl)
        {
            var customEntityRoute = new CustomEntityRoute()
            {
                UrlSlug = urlSlug
            };
            
            var url = _rule.MakeUrl(_pageRoute, customEntityRoute);

            url.Should().Be(expectedUrl);
        }
    }
}
