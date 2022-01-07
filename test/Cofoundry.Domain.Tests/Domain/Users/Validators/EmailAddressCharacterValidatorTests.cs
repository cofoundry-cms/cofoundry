using Cofoundry.Domain.Internal;
using FluentAssertions;
using Xunit;

namespace Cofoundry.Domain.Tests.Users.Validators
{
    public class EmailAddressCharacterValidatorTests
    {
        [Theory]
        [InlineData("abC123@abc.example.com", "abCc.exmplo")]
        [InlineData("1-33-44@[!<>]", "-[!<>]")]
        public void WhenAllowsOnlyDigit_ReturnsNonDigits(string email, string expected)
        {
            var options = new EmailAddressOptions()
            {
                AdditionalAllowedCharacters = null,
                AllowAnyCharacter = false,
                AllowAnyLetter = false,
                AllowAnyDigit = true
            };

            var result = EmailAddressCharacterValidator.GetInvalidCharacters(email, options);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("abC123@abc.example.com", "123.")]
        [InlineData("δοκιμή-[121!]@παράδειγμα", "-[12!]")]
        [InlineData("1-33-44@[!<>]", "1-34[!<>]")]
        public void WhenAllowsOnlyLetters_ReturnsNonLetters(string email, string expected)
        {
            var options = new EmailAddressOptions()
            {
                AdditionalAllowedCharacters = null,
                AllowAnyCharacter = false,
                AllowAnyLetter = true,
                AllowAnyDigit = false
            };

            var result = EmailAddressCharacterValidator.GetInvalidCharacters(email, options);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("celeriac@example.com", "elrixmpo")]
        [InlineData("123@example.com", "123exmplo")]
        [InlineData("1-33-44@[!<>]", "1-34[!<>]")]
        [InlineData("δοκιμή-[121!]@παράδειγμα", "δοκιμή-[12!]παράεγ")]
        public void WhenAllowsSpecificChars_ReturnInvalid(string email, string expected)
        {
            var options = new EmailAddressOptions()
            {
                AdditionalAllowedCharacters = "cat.",
                AllowAnyCharacter = false,
                AllowAnyLetter = false,
                AllowAnyDigit = false
            };

            var result = EmailAddressCharacterValidator.GetInvalidCharacters(email, options);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("abc@example.com")]
        [InlineData("abc.xyz@example.org")]
        [InlineData("abc-xyz.+123@example.com")]
        [InlineData("q@test-123.test.com")]
        [InlineData("abc/xyz@test.com")]
        [InlineData("admin!test@test")]
        [InlineData("user%example.com@example.com")]
        [InlineData("u-@example.com")]
        [InlineData("Müller@müller.example.com")]
        [InlineData("δοκιμή@παράδειγμα")]
        [InlineData("試験@試験.日本")]
        public void UsingDefaultRules_AcceptsValidEmails(string email)
        {
            var options = new EmailAddressOptions()
            {
                AllowAnyCharacter = false
            };
            var result = EmailAddressCharacterValidator.GetInvalidCharacters(email, options);

            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData("\"  \"@example.com")]
        [InlineData("\"test...test\"@example.com")]
        [InlineData("postmaster@[127.0.0.1]")]
        [InlineData("(test)abc@example.com")]
        public void UsingDefaultRules_IrregularEmailsNotAllowed(string email)
        {
            var options = new EmailAddressOptions()
            {
                AllowAnyCharacter = false
            };
            var result = EmailAddressCharacterValidator.GetInvalidCharacters(email, options);

            result.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Theory]
        [InlineData("\"  \"@example.com")]
        [InlineData("\"test...test\"@example.com")]
        [InlineData("postmaster@[127.0.0.1]")]
        [InlineData("(test)abc@example.com")]
        public void AllowAny_AcceptsAnything(string email)
        {
            var options = new EmailAddressOptions()
            {
                AllowAnyCharacter = true
            };
            var result = EmailAddressCharacterValidator.GetInvalidCharacters(email, options);

            result.Should().BeEmpty();
        }
    }
}
