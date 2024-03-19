using Cofoundry.Core.Validation;
using Cofoundry.Domain.Internal;
using NSubstitute;

namespace Cofoundry.Domain.Tests.PasswordPolicies.Models;

public class PasswordPolicyTests
{
    [Fact]
    public async Task ValidateAsync_AggregatesOnlySyncErrors()
    {
        var validators = new List<INewPasswordValidatorBase>();
        var asyncValidationError = new ValidationError("test-async");

        var mockAsyncValidator = Substitute.For<IAsyncNewPasswordValidator>();
        mockAsyncValidator
            .ValidateAsync(Arg.Any<INewPasswordValidationContext>())
            .Returns(asyncValidationError);
        validators.Add(mockAsyncValidator);

        var maxLengthValidator = new MaxLengthNewPasswordValidator();
        maxLengthValidator.Configure(6);
        validators.Add(maxLengthValidator);

        var uniqueCharsValidator = new MinUniqueCharactersNewPasswordValidator();
        uniqueCharsValidator.Configure(5);
        validators.Add(uniqueCharsValidator);

        var policy = new PasswordPolicy("test", validators, new Dictionary<string, string>());
        var context = new NewPasswordValidationContext()
        {
            Password = "aaaaaaaaa"
        };

        var errors = await policy.ValidateAsync(context);

        errors.Should().HaveCount(2);
        errors.Should().NotContain(asyncValidationError);
    }
}
