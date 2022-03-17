namespace Cofoundry.Domain;

/// <summary>
/// Prevents groups of repeated characters in a password by validating that a minimum number of 
/// unique characters e.g. the password "YYZ-YYZ-YYZ" contains 3 unique characters.
/// </summary>
/// <inheritdoc/>
public class MinUniqueCharactersNewPasswordValidator
    : INewPasswordValidator
    , INewPasswordValidatorWithConfig<int>
{
    /// <summary>
    /// The inclusive minimum number of unique characters to allow e.g. if the minimum was 4 then
    /// "abcabcabcabc" would be an invalid password.
    /// </summary>
    public int MinUniqueCharacters { get; private set; }

    public void Configure(int minUniqueCharacters)
    {
        if (minUniqueCharacters > PasswordOptions.MAX_LENGTH_BOUNDARY) throw new ArgumentOutOfRangeException(nameof(minUniqueCharacters));

        MinUniqueCharacters = minUniqueCharacters;
    }

    public string Criteria => $"Must have at least {MinUniqueCharacters} unique characters.";

    public ValidationError Validate(INewPasswordValidationContext context)
    {
        if (MinUniqueCharacters > 1 && context
            .Password
            .Distinct()
            .Count() < MinUniqueCharacters)
        {
            return PasswordPolicyValidationErrors
                .MinUniqueCharacters
                .Customize()
                .WithMessageFormatParameters(MinUniqueCharacters)
                .WithProperties(context.PropertyName)
                .Create();
        }

        return null;
    }
}
