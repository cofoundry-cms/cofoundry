namespace Cofoundry.Domain;

/// <summary>
/// Validates that a password is not a sequence, or reverse sequence of characters
/// e.g. "abcde", "54321". This is implemented simply by comparing the numeric value
/// of unicode character codes to principally support latin characters, but other 
/// character sequences may also be detected.
/// </summary>
/// <inheritdoc/>
public class NotSequentialNewPasswordValidator : INewPasswordValidator
{
    public string Criteria => $"Must not be a sequence of numbers or characters.";

    public ValidationError Validate(INewPasswordValidationContext context)
    {
        var lowerPassword = context
            .Password
            .ToLowerInvariant();

        if (IsCodeSequence(lowerPassword))
        {
            return PasswordPolicyValidationErrors.NotSequential.Create(context.PropertyName);
        }

        return null;
    }

    private bool IsCodeSequence(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;

        var isSequence = true;
        var isReverseSequence = true;

        char lastChar = password[0];

        for (int i = 1; i < password.Length; i++)
        {
            if (lastChar != (password[i] + 1)) isReverseSequence = false;
            if (lastChar != (password[i] - 1)) isSequence = false;

            if (!isSequence && !isReverseSequence) return false;
            lastChar = password[i];
        }

        return isReverseSequence || isSequence;
    }
}
