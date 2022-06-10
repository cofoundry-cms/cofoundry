namespace Cofoundry.Core.Tests.Core.Exceptions;

public class ArgumentEmptyExceptionTests
{
    [Fact]
    public void ThrowIfNullOrWhitespace_WhenNull_ThrowsArgumentNullException()
    {
        string testValue = null;

        Action act = () => ArgumentEmptyException.ThrowIfNullOrWhitespace(testValue);

        act.Should().Throw<ArgumentNullException>().WithMessage("*'testValue'*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ThrowIfNullOrWhitespace_WhenNull_ThrowsArgumentEmptyException(string testValue)
    {
        Action act = () => ArgumentEmptyException.ThrowIfNullOrWhitespace(testValue);

        act.Should().Throw<ArgumentEmptyException>().WithMessage("*'testValue'*");
    }

    [Fact]
    public void ThrowIfNullOrWhitespace_WhenNotNullOrEmpty_DoesNotThrow()
    {
        var testValue = "inconceivable!";

        Action act = () => ArgumentEmptyException.ThrowIfNullOrWhitespace(testValue);

        act.Should().NotThrow();
    }

    [Fact]
    public void ThrowIfDefault_WhenDefault_ThrowsArgumentNullException()
    {
        var testValue = new DateTime();

        Action act = () => ArgumentEmptyException.ThrowIfDefault(testValue);

        act.Should().Throw<ArgumentEmptyException>().WithMessage("*'testValue'*");
    }

    [Fact]
    public void ThrowIfDefault_WhenNotDefault_DoesNotThrow()
    {
        DateTime testValue = DateTime.MaxValue;

        Action act = () => ArgumentEmptyException.ThrowIfDefault(testValue);

        act.Should().NotThrow();
    }
}
