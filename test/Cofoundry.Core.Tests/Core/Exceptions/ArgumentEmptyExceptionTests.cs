namespace Cofoundry.Core.Tests.Core.Exceptions;

public class ArgumentEmptyExceptionTests
{
    [Fact]
    public void ThrowIfNullOrEmpty_WhenNull_ThrowsArgumentNullException()
    {
        string[]? testValue = null;

        Action act = () => ArgumentEmptyException.ThrowIfNullOrEmpty(testValue);

        act.Should().Throw<ArgumentNullException>().WithMessage("*'testValue'*");
    }

    [Fact]
    public void ThrowIfNullOrEmpty_WhenEmpty_ThrowsArgumentEmptyException()
    {
        string[]? testValue = [];
        Action act = () => ArgumentEmptyException.ThrowIfNullOrEmpty(testValue);

        act.Should().Throw<ArgumentEmptyException>().WithMessage("*'testValue'*");
    }

    [Fact]
    public void ThrowIfNullOrEmpty_WhenNotNullOrEmpty_DoesNotThrow()
    {
        string[]? testValue = ["inconceivable!"];

        Action act = () => ArgumentEmptyException.ThrowIfNullOrEmpty(testValue);

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
        var testValue = DateTime.MaxValue;

        Action act = () => ArgumentEmptyException.ThrowIfDefault(testValue);

        act.Should().NotThrow();
    }
}
