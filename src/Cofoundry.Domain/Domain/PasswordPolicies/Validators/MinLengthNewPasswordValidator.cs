namespace Cofoundry.Domain;

/// <summary>
/// Validate that a password meets a minimum length. See
/// https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html#implement-proper-password-strength-controls
/// for up to date information on best practice password lengths.
/// </summary>
/// <inheritdoc/>
public class MinLengthNewPasswordValidator
    : INewPasswordValidator
    , INewPasswordValidatorWithConfig<int>
{
    /// <summary>
    /// The inclusive minimum length that the password should be. Must be between 6 and 2048
    /// characters.
    /// </summary>
    public int MinLength { get; private set; }

    public void Configure(int minLength)
    {
        if (minLength < PasswordOptions.MIN_LENGTH_BOUNDARY) throw new ArgumentOutOfRangeException(nameof(minLength));
        if (minLength > PasswordOptions.MAX_LENGTH_BOUNDARY) throw new ArgumentOutOfRangeException(nameof(minLength));

        MinLength = minLength;
    }

    public string Criteria => $"Must be at least {MinLength} characters.";

    public ValidationError Validate(INewPasswordValidationContext context)
    {
        if (MinLength == 0) throw new InvalidOperationException($"{nameof(Configure)} has not been called.");

        if (context.Password.Length < MinLength)
        {
            return PasswordPolicyValidationErrors
                .MinLengthNotMet
                .Customize()
                .WithMessageFormatParameters(MinLength)
                .WithProperties(context.PropertyName)
                .Create();
        }

        return null;
    }
}
