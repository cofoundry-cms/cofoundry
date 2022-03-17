using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain;

public static class IPasswordPolicyBuilderExtensions
{
    /// <summary>
    /// Adds the Cofoundry defaults to the policy, making use of any settings
    /// defined in <see cref="PasswordOptions"/> through configuration sources 
    /// e.g. appsettings.json.
    /// </summary>
    public static IPasswordPolicyBuilder UseDefaults(this IPasswordPolicyBuilder builder)
    {
        return builder.UseDefaults(null);
    }

    /// <summary>
    /// Adds the Cofoundry defaults to the policy, using the specified 
    /// options transformer to layer additional configuration on top of
    /// those defined in configuration sources e.g. appsettings.json.
    /// </summary>
    /// <param name="passwordOptionsConfig">
    /// Transformer to configure any additional settings on top of the
    /// default options.
    /// </param>
    public static IPasswordPolicyBuilder UseDefaults(
        this IPasswordPolicyBuilder builder,
        Action<PasswordOptions> passwordOptionsConfig = null
        )
    {
        var options = GetPasswordOptions(builder);

        if (passwordOptionsConfig != null)
        {
            options = options.Clone();
            passwordOptionsConfig(options);
        }

        builder
            .SetDescription($"Passwords must be between {options.MinLength} and {options.MaxLength} characters.")
            .ValidateMinLength(options.MinLength)
            .ValidateMaxLength(options.MaxLength)
            .ValidateMinUniqueCharacters(options.MinUniqueCharacters)
            .ValidateNotCurrentPassword()
            .ValidateNotPersonalData()
            .ValidateNotSequential()
            ;

        return builder;
    }

    /// <summary>
    /// Adds an attribute to the policy. Attributes are loosely mapped to
    /// HTML attributes for password inputs. Expressing these attributes here allows
    /// a developer to dynamically map policy requirements to an input field. The
    /// <see cref="PasswordPolicyAttributes"/> constants can be used as keys for known
    /// attributes.
    /// </summary>
    /// <param name="attribute">
    /// The attribute name e.g. "minlength". The  <see cref="PasswordPolicyAttributes"/> constants 
    /// can be used for known attribute keys.
    /// </param>
    /// <param name="value">The value of the attribute as an integer e.g. 10.</param>
    public static IPasswordPolicyBuilder AddAttribute(this IPasswordPolicyBuilder builder, string attribute, int value)
    {
        return builder.AddAttribute(attribute, value.ToString());
    }

    /// <summary>
    /// Validate that a password meets a minimum length. See
    /// https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html#implement-proper-password-strength-controls
    /// for up to date information on best practice password lengths.
    /// </summary>
    /// <param name="minLength">
    /// The inclusive minimum length that the password should be. Must be between 6 and 2048
    /// characters.
    /// </param>
    public static IPasswordPolicyBuilder ValidateMinLength(this IPasswordPolicyBuilder builder, int minLength)
    {
        builder
            .AddValidatorWithConfig<MinLengthNewPasswordValidator, int>(minLength)
            .AddAttribute(PasswordPolicyAttributes.MinLength, minLength);

        return builder;
    }

    /// <summary>
    /// Validate that a password meets a maximum length. See
    /// https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html#implement-proper-password-strength-controls
    /// for up to date information on best practice password lengths.
    /// </summary>
    /// <param name="maxLength">
    /// The inclusive maximum length that the password should be. Must be between 6 and 2048
    /// characters.
    /// </param>
    public static IPasswordPolicyBuilder ValidateMaxLength(this IPasswordPolicyBuilder builder, int maxLength)
    {
        builder
            .AddValidatorWithConfig<MaxLengthNewPasswordValidator, int>(maxLength)
            .AddAttribute(PasswordPolicyAttributes.MaxLength, maxLength);

        return builder;
    }

    /// <summary>
    /// Prevents groups of repeated characters in a password by validating that a minimum number of 
    /// unique characters e.g. the password "YYZ-YYZ-YYZ" contains 3 unique characters.
    /// </summary>
    /// <param name="minUniqueCharacters">
    /// The inclusive minimum number of unique characters to allow e.g. if the minimum was 4 then
    /// "abcabcabcabc" would be an invalid password.
    /// </param>
    public static IPasswordPolicyBuilder ValidateMinUniqueCharacters(this IPasswordPolicyBuilder builder, int minUniqueCharacters)
    {
        builder.AddValidatorWithConfig<MinUniqueCharactersNewPasswordValidator, int>(minUniqueCharacters);

        return builder;
    }

    /// <summary>
    /// Validates that a password does not match either their username
    /// or email address, based on a case-insenstivie equality comparison.
    /// </summary>
    public static IPasswordPolicyBuilder ValidateNotPersonalData(this IPasswordPolicyBuilder builder)
    {
        builder.AddValidator<NotBePersonalDataNewPasswordValidator>();

        return builder;
    }

    /// <summary>
    /// Validates that a password does not exactly match a users current password
    /// if one is supplied. A current password is only supplied when a password
    /// is authenticated and changed in the same command and therefore is only
    /// validated in these conditions.
    /// </summary>
    public static IPasswordPolicyBuilder ValidateNotCurrentPassword(this IPasswordPolicyBuilder builder)
    {
        builder.AddValidator<NotCurrentPasswordNewPasswordValidator>();

        return builder;
    }

    /// <summary>
    /// Validates that a password is not a sequence or reverse-sequence of characters
    /// e.g. "abcdefghij" or "987654321". This is case-sensitive and is done simply
    /// by comparing character codes.
    /// </summary>
    public static IPasswordPolicyBuilder ValidateNotSequential(this IPasswordPolicyBuilder builder)
    {
        builder.AddValidator<NotSequentialNewPasswordValidator>();

        return builder;
    }

    private static PasswordOptions GetPasswordOptions(IPasswordPolicyBuilder builder)
    {
        var extendableBuilder = builder.AsExtendable();

        return extendableBuilder.Options;
    }
}
