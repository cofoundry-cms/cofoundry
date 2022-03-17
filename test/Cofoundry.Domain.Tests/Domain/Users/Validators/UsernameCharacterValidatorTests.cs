using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Tests.Users.Validators;

public class UsernameCharacterValidatorTests
{
    [Theory]
    [InlineData("abcD 123", "abcD ")]
    [InlineData("1-33-44@[!<>]", "-@[!<>]")]
    public void WhenAllowsOnlyDigit_ReturnsNonDigits(string email, string expected)
    {
        var options = new UsernameOptions()
        {
            AdditionalAllowedCharacters = null,
            AllowAnyCharacter = false,
            AllowAnyLetter = false,
            AllowAnyDigit = true
        };

        var result = UsernameCharacterValidator.GetInvalidCharacters(email, options);

        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("abcD 123", " 123")]
    [InlineData("δοκιμή-[121!]", "-[12!]")]
    [InlineData("1-33-44@[!<>]", "1-34@[!<>]")]
    public void WhenAllowsOnlyLetters_ReturnsNonLetters(string email, string expected)
    {
        var options = new UsernameOptions()
        {
            AdditionalAllowedCharacters = null,
            AllowAnyCharacter = false,
            AllowAnyLetter = true,
            AllowAnyDigit = false
        };

        var result = UsernameCharacterValidator.GetInvalidCharacters(email, options);

        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("Wuffles", "Wufles")]
    [InlineData("Dog123", "D123")]
    [InlineData("1-33-44@[!<>]", "1-34@[!<>]")]
    [InlineData("δοκιμή-[121!] παράδειγμα", "δοκιμή-[12!] παράεγ")]
    public void WhenAllowsSpecificChars_ReturnInvalid(string email, string expected)
    {
        var options = new UsernameOptions()
        {
            AdditionalAllowedCharacters = "dog",
            AllowAnyCharacter = false,
            AllowAnyLetter = false,
            AllowAnyDigit = false
        };

        var result = UsernameCharacterValidator.GetInvalidCharacters(email, options);

        result.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData("Su 90")]
    [InlineData("Jim O'Reilly")]
    [InlineData("test-example")]
    [InlineData("ALLCAPS_4EVER")]
    [InlineData("dotty.dot-face")]
    [InlineData("Müller")]
    [InlineData("δοκιμή")]
    [InlineData("試験 試験")]
    public void UsingDefaultRules_AcceptsValidUsernames(string email)
    {
        var options = new UsernameOptions()
        {
            AllowAnyCharacter = false
        };
        var result = UsernameCharacterValidator.GetInvalidCharacters(email, options);

        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("<blink>O</blink>")]
    [InlineData("🦔")]
    [InlineData("abc xyz!")]
    [InlineData("Example (test)")]
    public void UsingDefaultRules_IrregularEmailsNotAllowed(string email)
    {
        var options = new UsernameOptions()
        {
            AllowAnyCharacter = false
        };
        var result = UsernameCharacterValidator.GetInvalidCharacters(email, options);

        result.Should().HaveCountGreaterOrEqualTo(1);
    }

    [Theory]
    [InlineData("<blink>O</blink>")]
    [InlineData("🦔")]
    [InlineData("abc xyz!")]
    [InlineData("Example (test)")]
    public void AllowAny_AcceptsAnything(string email)
    {
        var options = new UsernameOptions()
        {
            AllowAnyCharacter = true
        };
        var result = UsernameCharacterValidator.GetInvalidCharacters(email, options);

        result.Should().BeEmpty();
    }
}
