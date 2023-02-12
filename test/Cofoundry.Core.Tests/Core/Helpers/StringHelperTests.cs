namespace Cofoundry.Core.Tests;

public class StringHelperTests
{
    [Fact]
    public void RemovePrefix_WhenNull_ReturnsNull()
    {
        var result = StringHelper.RemovePrefix(null, "x");

        result.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("AB CD E")]
    [InlineData("left-right")]
    public void RemovePrefix_WhenNoPrefixFound_ReturnsInput(string input)
    {
        var result = StringHelper.RemovePrefix(input, "x");

        Assert.Equal(result, input);
    }

    [Theory]
    [InlineData("ABCDEF", "DEF")]
    [InlineData("abcDEF", "abcDEF")]
    public void RemovePrefix_CanRemoveCaseSensitivePrefix(string input, string expected)
    {
        var result = StringHelper.RemovePrefix(input, "ABC");

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("ABCDEF")]
    [InlineData("abcDEF")]
    public void RemovePrefix_CanRemoveCaseInsensitivePrefix(string input)
    {
        var result = StringHelper.RemovePrefix(input, "ABC", StringComparison.OrdinalIgnoreCase);

        Assert.Equal("DEF", result);
    }
}
