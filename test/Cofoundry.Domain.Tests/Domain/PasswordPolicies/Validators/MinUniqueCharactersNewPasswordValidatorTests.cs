using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Tests.UserAreas.Configuration.NewPasswordValidation.Validators;

public class MinUniqueCharactersNewPasswordValidatorTests
{
    [Theory]
    [InlineData("abcdefghijklmnop")]
    [InlineData("123")]
    [InlineData("aaabbbccc")]
    public void WhenMoreThanMin_ReturnsSuccess(string password)
    {
        var validator = new MinUniqueCharactersNewPasswordValidator();
        validator.Configure(3);

        var context = new NewPasswordValidationContext()
        {
            Password = password
        };

        var result = validator.Validate(context);

        result.Should().BeNull();
    }

    [Theory]
    [InlineData("a")]
    [InlineData("12")]
    [InlineData("abababababababababab")]
    public void WhenLessThanMin_ReturnsError(string password)
    {
        var validator = new MinUniqueCharactersNewPasswordValidator();
        validator.Configure(3);

        var context = new NewPasswordValidationContext()
        {
            Password = password
        };

        var result = validator.Validate(context);

        result.Should().NotBeNull();
        result.ErrorCode.Should().Be(PasswordPolicyValidationErrors.MinUniqueCharacters.ErrorCode);
    }
}
