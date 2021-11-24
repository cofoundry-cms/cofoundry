using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Domain.Tests.Domain.CustomEntities.RoutingRules
{
    public class IdCustomEntityRoutingRuleTests
    {
        const string PATH = "/my-example/path/";
        private readonly IdCustomEntityRoutingRule _rule;
        private readonly PageRoute _pageRoute;

        public IdCustomEntityRoutingRuleTests()
        {
            _rule = new IdCustomEntityRoutingRule();
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
        [InlineData("1")]
        [InlineData("123456778")]
        [InlineData("021")]
        public void MatchesRule_ValidId_ReturnsTrue(string customEntityId)
        {
            var url = PATH + customEntityId;

            var isMatch = _rule.MatchesRule(url, _pageRoute);

            isMatch.Should().BeTrue();
        }

        [Theory]
        [InlineData("0")]
        [InlineData("not-an-int")]
        [InlineData("q")]
        public void MatchesRule_WhenInvalid_ReturnsFalse(string urlPart)
        {
            var url = PATH + urlPart;

            var isMatch = _rule.MatchesRule(url, _pageRoute);

            isMatch.Should().BeFalse();
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("123456778", 123456778)]
        [InlineData("021", 21)]
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
        [InlineData(1, "/my-example/path/1")]
        [InlineData(123456778, "/my-example/path/123456778")]
        public void MakeUrl(int customEntityId, string expectedUrl)
        {
            var customEntityRoute = new CustomEntityRoute()
            {
                CustomEntityId = customEntityId
            };
            
            var url = _rule.MakeUrl(_pageRoute, customEntityRoute);

            url.Should().Be(expectedUrl);
        }
    }
}
