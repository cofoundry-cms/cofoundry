using Cofoundry.Domain.Internal;
using FluentAssertions;
using Xunit;

namespace Cofoundry.Domain.Tests.PasswordPolicies.Validators
{
    public class NotCurrentPasswordNewPasswordValidatorTests
    {
        [Fact]
        public void WhenNotCurrant_ReturnsSuccess()
        {
            var validator = new NotCurrentPasswordNewPasswordValidator();
            var context = new NewPasswordValidationContext()
            {
                Password = "goosberry",
                CurrentPassword = "raisin"
            };

            var result = validator.Validate(context);

            result.Should().BeNull();
        }

        [Fact]
        public void WhenCurrent_ReturnsError()
        {
            var validator = new NotCurrentPasswordNewPasswordValidator();
            var context = new NewPasswordValidationContext()
            {
                Password = "ac",
                CurrentPassword = "ac"
            };

            var result = validator.Validate(context);

            result.Should().NotBeNull();
            result.ErrorCode.Should().Be("cf-new-password-not-current-password");
        }
    }
}
