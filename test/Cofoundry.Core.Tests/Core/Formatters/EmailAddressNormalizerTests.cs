using Cofoundry.Core.Internal;
using FluentAssertions;
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
        public void Normalize_LowercasesDomainOnly(string email, string expected)
        {
            var result = _emailAddressNormalizer.Normalize(email);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("μαρια@μαρια.gr", "μαρια@μαρια.gr")]
        [InlineData("EXAMPLE@ονομα.gr", "EXAMPLE@ονομα.gr")]
        [InlineData("example@😉.fm", "example@😉.fm")]
        [InlineData("example@Σαμαράς.ελ ", "example@Σαμαράς.ελ")]
        [InlineData(" example@Γιώργος.ελ", "example@Γιώργος.ελ")]
        [InlineData(" MÜller@MÜller.example.com", "MÜller@MÜller.example.com")]
        public void Normalize_IDNDomain_Ignores(string email, string expected)
        {
            var result = _emailAddressNormalizer.Normalize(email);

            result.Should().Be(expected);
        }
    }
}
