using Cofoundry.Core.Extendable;

namespace Cofoundry.Core.Tests.Core.Formatters;

public class UsernameUniquifierTests
{
    private readonly UsernameUniquifier _usernameUniquifier = new(new UsernameNormalizer());

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void WhenInvalid_ReturnsNull(string? email)
    {
        var result = _usernameUniquifier.Uniquify(email);

        result.Should().BeNull();
    }

    [Fact]
    public void Lowercases()
    {
        var result = _usernameUniquifier.Uniquify(" D Angel ");

        result.Should().Be("d angel");
    }
}
