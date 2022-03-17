using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Tests.PasswordPolicies.Validators;

public class MinLengthNewPasswordValidatotTests
{
    [Theory]
    [InlineData("abcdefghijklmnop")]
    [InlineData("12345678")]
    public void WhenMoreThanMinLength_ReturnsSuccess(string password)
    {
        var validator = new MinLengthNewPasswordValidator();
        validator.Configure(8);

        var context = new NewPasswordValidationContext()
        {
            Password = password
        };

        var result = validator.Validate(context);

        result.Should().BeNull();
    }

    [Fact]
    public void WhenLessThanMinLength_ReturnsError()
    {
        var validator = new MinLengthNewPasswordValidator();
        validator.Configure(8);

        var context = new NewPasswordValidationContext()
        {
            Password = "1234567"
        };

        var result = validator.Validate(context);

        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(PasswordPolicyValidationErrors.MinLengthNotMet.ErrorCode);
    }
}
