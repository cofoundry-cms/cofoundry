using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cofoundry.Core.Tests
{
    public class HttpUriParserTests
    {
        #region ParseAbsoluteOrDefault

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void ParseAbsoluteOrDefault_WhenNullOrEmpty_ReturnsNull(string input)
        {
            var result = HttpUriParser.ParseAbsoluteOrDefault(input);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("www.cofoundry.org", "http://www.cofoundry.org")]
        [InlineData("uberdigital.com", "http://uberdigital.com")]
        [InlineData("www.cofoundry.org/blog", "http://www.cofoundry.org/blog")]
        public void ParseAbsoluteOrDefault_WithoutScheme_CanParse(string input, string expected)
        {
            var result = HttpUriParser.ParseAbsoluteOrDefault(input);
            var expectedUri = new Uri(expected);
            Assert.Equal(expectedUri, result);
            Assert.True(result.IsAbsoluteUri);
        }

        [Theory]
        [InlineData("http://www.cofoundry.org")]
        [InlineData("http://uberdigital.com")]
        [InlineData("https://www.cofoundry.org")]
        [InlineData("https://uberdigital.com")]
        public void ParseAbsoluteOrDefault_WithHttpScheme_CanParse(string input)
        {
            var result = HttpUriParser.ParseAbsoluteOrDefault(input);
            var expectedUri = new Uri(input);
            Assert.Equal(expectedUri, result);
            Assert.True(result.IsAbsoluteUri);
        }

        [Theory]
        [InlineData("ftp://www.cofoundry.org")]
        [InlineData("smtp://uberdigital.com")]
        [InlineData("gopher://www.cofoundry.org")]
        public void ParseAbsoluteOrDefault_WithInvalidScheme_ReturnsNull(string input)
        {
            var result = HttpUriParser.ParseAbsoluteOrDefault(input);
            Assert.Null(result);
        }

        [Theory]
        [InlineData("/pages.aspx")]
        [InlineData("invalid text")]
        [InlineData("../404.htm")]
        public void ParseAbsoluteOrDefault_WhenNotAbsolute_ReturnsNull(string input)
        {
            var result = HttpUriParser.ParseAbsoluteOrDefault(input);
            Assert.Null(result);
        }

        #endregion

        #region ParseOrDefault

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public void ParseOrDefault_WhenNullOrEmpty_ReturnsNull(string input)
        {
            var result = HttpUriParser.ParseOrDefault(input);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("www.cofoundry.org", "http://www.cofoundry.org")]
        [InlineData("uberdigital.com", "http://uberdigital.com")]
        [InlineData("www.cofoundry.org/blog", "http://www.cofoundry.org/blog")]
        public void ParseOrDefault_WithoutScheme_CanParse(string input, string expected)
        {
            var result = HttpUriParser.ParseOrDefault(input);
            var expectedUri = new Uri(expected);
            Assert.Equal(expectedUri, result);
            Assert.True(result.IsAbsoluteUri);
        }

        [Theory]
        [InlineData("http://www.cofoundry.org")]
        [InlineData("http://uberdigital.com")]
        [InlineData("https://www.cofoundry.org")]
        [InlineData("https://uberdigital.com")]
        public void ParseOrDefault_WithHttpScheme_CanParse(string input)
        {
            var result = HttpUriParser.ParseOrDefault(input);
            var expectedUri = new Uri(input);
            Assert.Equal(expectedUri, result);
            Assert.True(result.IsAbsoluteUri);
        }

        [Theory]
        [InlineData("ftp://www.cofoundry.org")]
        [InlineData("smtp://uberdigital.com")]
        [InlineData("gopher://www.cofoundry.org")]
        public void ParseOrDefault_WithInvalidScheme_ReturnsNull(string input)
        {
            var result = HttpUriParser.ParseOrDefault(input);
            Assert.Null(result);
        }

        [Theory]
        [InlineData("/pages.aspx")]
        [InlineData("invalid text")]
        [InlineData("../404.htm")]
        public void ParseOrDefault_WhenNotAbsolute_ParsesAsRelative(string input)
        {
            var result = HttpUriParser.ParseOrDefault(input);
            var expectedUri = new Uri(input, UriKind.Relative);
            Assert.Equal(expectedUri, result);
            Assert.True(!result.IsAbsoluteUri);
        }

        #endregion
    }
}
