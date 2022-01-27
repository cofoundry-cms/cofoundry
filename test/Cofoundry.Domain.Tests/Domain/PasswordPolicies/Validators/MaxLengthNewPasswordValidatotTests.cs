using Cofoundry.Domain.Internal;
using FluentAssertions;
using Xunit;

namespace Cofoundry.Domain.Tests.PasswordPolicies.Validators
{
    public class MaxLengthNewPasswordValidatotTests
    {
        [Theory]
        [InlineData("abc")]
        [InlineData("12345678")]
        public void WhenLessThanMaxLength_ReturnsSuccess(string password)
        {
            var validator = new MaxLengthNewPasswordValidator();
            validator.Configure(8);

            var context = new NewPasswordValidationContext()
            {
                Password = password
            };

            var result = validator.Validate(context);

            result.Should().BeNull();
        }

        [Fact]
        public void WhenMoreThanMaxLength_ReturnsError()
        {
            var validator = new MaxLengthNewPasswordValidator();
            validator.Configure(8);

            var context = new NewPasswordValidationContext()
            {
                Password = "123456789"
            };

            var result = validator.Validate(context);

            result.Should().NotBeNull();
            result.ErrorCode.Should().Be(PasswordPolicyValidationErrors.MaxLengthExceeded.ErrorCode);
        }
    }
}
