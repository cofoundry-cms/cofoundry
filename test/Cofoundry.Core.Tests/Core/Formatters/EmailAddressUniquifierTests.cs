using Cofoundry.Core.Extendable;

namespace Cofoundry.Core.Tests.Core.Formatters;

public class EmailAddressUniquifierTests
{
    private EmailAddressUniquifier _emailAddressUniquifier = new EmailAddressUniquifier(new EmailAddressNormalizer());

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   @   ")]
    [InlineData("@@@")]
    [InlineData("@example.com")]
    [InlineData("example@")]
    public void WhenInvalid_ReturnsNull(string email)
    {
        var result = _emailAddressUniquifier.Uniquify(email);

        result.Should().BeNull();
    }

    [Theory]
    [InlineData("Example@example.com", "example@example.com")]
    [InlineData("EXAMPLE@EXAMPLE.com", "example@example.com")]
    [InlineData("example+1234@example.COM", "example+1234@example.com")]
    [InlineData("EXAMPLE@😉.FM", "example@😉.fm")]
    [InlineData(" MÜller@MÜller.example.Com", "müller@müller.example.com")]
    [InlineData("μαρια@μαρια.GR", "μαρια@μαρια.gr")]
    public void Lowercases(string email, string expected)
    {
        var result = _emailAddressUniquifier.Uniquify(email);

        result.Should().Be(expected);
    }
}
