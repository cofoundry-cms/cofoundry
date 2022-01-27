using Cofoundry.Domain.Internal;
using FluentAssertions;
using Xunit;

namespace Cofoundry.Domain.Tests.PasswordPolicies.Validators
{
    public class NotSequentialNewPasswordValidatorTests
    {
        [Theory]
        [InlineData("abab")]
        [InlineData("a1234567")]
        [InlineData("abcde1")]
        public void WhenNotSequential_ReturnsSuccess(string password)
        {
            var validator = new NotSequentialNewPasswordValidator();
            var context = new NewPasswordValidationContext()
            {
                Password = password
            };

            var result = validator.Validate(context);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData("abcdefgHIJkL")]
        [InlineData("0123456789")]
        [InlineData("ZYXWVU")]
        [InlineData("9876543210")]
        public void WhenSequential_ReturnsError(string password)
        {
            var validator = new NotSequentialNewPasswordValidator();
            var context = new NewPasswordValidationContext()
            {
                Password = password
            };

            var result = validator.Validate(context);

            result.Should().NotBeNull();
            result.ErrorCode.Should().Be(PasswordPolicyValidationErrors.NotSequential.ErrorCode);
        }
    }
}
