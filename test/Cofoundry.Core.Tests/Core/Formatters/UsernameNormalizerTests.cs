using Cofoundry.Core.Extendable;
using FluentAssertions;
using Xunit;

namespace Cofoundry.Core.Tests.Core.Formatters
{
    public class UsernameNormalizerTests
    {
        private UsernameNormalizer _usernameNormalizer = new UsernameNormalizer();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void WhenInvalid_ReturnsNull(string email)
        {
            var result = _usernameNormalizer.Normalize(email);

            result.Should().BeNull();
        }

        [Fact]
        public void Trims()
        {
            var result = _usernameNormalizer.Normalize(" D Angel ");

            result.Should().Be("D Angel");
        }
    }
}
