namespace Cofoundry.Domain;

/// <summary>
/// Validates that a password does not exactly match a users current password
/// if one is supplied. A current password is only supplied when a password
/// is authenticated and changed in the same command and therefore is only
/// validated in these conditions.
/// </summary>
/// <inheritdoc/>
public class NotCurrentPasswordNewPasswordValidator : INewPasswordValidator
{
    public string Criteria => "Must not be the same as your current password.";

    public ValidationError Validate(INewPasswordValidationContext context)
    {
        if (!string.IsNullOrWhiteSpace(context.CurrentPassword)
            && context.CurrentPassword == context.Password)
        {
            return PasswordPolicyValidationErrors.NotCurrentPassword.Create(context.PropertyName);
        }

        return null;
    }
}
