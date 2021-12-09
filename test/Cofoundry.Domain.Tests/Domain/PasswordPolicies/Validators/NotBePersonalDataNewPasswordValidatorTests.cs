using Cofoundry.Domain.Internal;
using FluentAssertions;
using Xunit;

namespace Cofoundry.Domain.Tests.PasswordPolicies.Validators
{
    public class NotBePersonalDataNewPasswordValidatorTests
    {
        [Fact]
        public void WhenNotEmail_ReturnsSuccess()
        {
            var validator = new NotBePersonalDataNewPasswordValidator();
            var context = new NewPasswordValidationContext()
            {
                Password = "test",
                Email = "test@example.com"
            };

            var result = validator.Validate(context);

            result.Should().BeNull();
        }

        [Fact]
        public void WhenNotUsername_ReturnsSuccess()
        {
            var validator = new NotBePersonalDataNewPasswordValidator();
            var context = new NewPasswordValidationContext()
            {
                Password = "test123",
                Username = "Test 123"
            };

            var result = validator.Validate(context);

            result.Should().BeNull();
        }

        [Fact]
        public void WhenEmail_ReturnsError()
        {
            var validator = new NotBePersonalDataNewPasswordValidator();
            var context = new NewPasswordValidationContext()
            {
                Password = "test@Example.com",
                Email = "test@example.com"
            };

            var result = validator.Validate(context);

            result.Should().NotBeNull();
            result.ErrorCode.Should().Be("cf-new-password-not-personal-data-email");
        }

        [Fact]
        public void WhenUsername_ReturnsError()
        {
            var validator = new NotBePersonalDataNewPasswordValidator();
            var context = new NewPasswordValidationContext()
            {
                Password = "test 123",
                Username = "Test 123"
            };

            var result = validator.Validate(context);

            result.Should().NotBeNull();
            result.ErrorCode.Should().Be("cf-new-password-not-personal-data-username");
        }
    }
}
