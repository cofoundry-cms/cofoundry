namespace Cofoundry.Domain;

/// <summary>
/// Constants for for the <see cref="IPasswordPolicy.Attributes"/>
/// keys that map to attributes on an HTML password input.
/// </summary>
public static class PasswordPolicyAttributes
{
    /// <summary>
    /// Maps to the "minlength" HTML attribute.
    /// </summary>
    public const string MinLength = "minlength";

    /// <summary>
    /// Maps to the "maxlength" HTML attribute.
    /// </summary>
    public const string MaxLength = "maxlength";

    /// <summary>
    /// Maps to the "placeholder" HTML attribute.
    /// </summary>
    public const string Placeholder = "placeholder";

    /// <summary>
    /// Maps to the "pattern" HTML attribute for regular expression validation.
    /// </summary>
    public const string Pattern = "pattern";

    /// <summary>
    /// Maps to the "passwordrules" HTML attribute. This attribute
    /// is not standard, more info can be found here: https://developer.apple.com/password-rules/
    /// </summary>
    public const string PasswordRules = "passwordrules";
}
