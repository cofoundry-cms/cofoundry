using Cofoundry.Core.Extendable;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Cofoundry.Core.Tests.Core.Formatters
{
    public class EmailAddressNormalizerTests
    {
        private EmailAddressNormalizer _emailAddressNormalizer = new EmailAddressNormalizer();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   @   ")]
        [InlineData("@@@")]
        [InlineData("@example.com")]
        [InlineData("example@")]
        public void Normalize_WhenInvalid_ReturnsNull(string email)
        {
            var result = _emailAddressNormalizer.Normalize(email);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData(" example@example.com ")]
        [InlineData("example@example.com ")]
        [InlineData("    example@example.com")]
        public void Normalize_Trims(string email)
        {
            var result = _emailAddressNormalizer.Normalize(email);

            result.Should().Be("example@example.com");
        }

        [Theory]
        [InlineData("Example@example.com", "Example@example.com")]
        [InlineData("EXAMPLE@EXAMPLE.com", "EXAMPLE@example.com")]
        [InlineData("example@example.COM", "example@example.com")]
        [InlineData(" eXample@example.COm", "eXample@example.com")]
        [InlineData(" MÜller@MÜller.example.Com", "MÜller@müller.example.com")]
        [InlineData("EXAMPLE@😉.FM", "EXAMPLE@😉.fm")]
        [InlineData("μαρια@μαρια.GR", "μαρια@μαρια.gr")]
        public void Normalize_LowercasesDomainOnly(string email, string expected)
        {
            var result = _emailAddressNormalizer.Normalize(email);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   @   ")]
        [InlineData("@@@")]
        [InlineData("@example.com")]
        [InlineData("example@")]
        public void NormalizeAsParts_WhenInvalid_ReturnsNull(string email)
        {
            var result = _emailAddressNormalizer.NormalizeAsParts(email);

            result.Should().BeNull();
        }

        [Fact]
        public void NormalizeAsParts_CanNormalize()
        {
            const string FORMATTED_RESULT = "MÜller@müller.example.com";
            var result = _emailAddressNormalizer.NormalizeAsParts(" MÜller@müller.example.com ");

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Local.Should().Be("MÜller");
                result.Domain.Should().NotBeNull();
                result.Domain.Name.Should().Be("müller.example.com");
                result.Domain.IdnName.Should().Be("xn--mller-kva.example.com");
                result.ToEmailAddress().Should().Be(FORMATTED_RESULT);
                result.ToString().Should().Be(FORMATTED_RESULT);
            }
        }
    }
}
