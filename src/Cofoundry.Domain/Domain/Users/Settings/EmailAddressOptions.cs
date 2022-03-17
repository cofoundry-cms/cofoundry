namespace Cofoundry.Domain;

/// <summary>
/// Options to control the formatting and validation of user email 
/// addresses.
/// </summary>
public class EmailAddressOptions
{
    /// <summary>
    /// Allows any character in an email, effectively bypassing characters
    /// validation. Defaults to <see langword="true"/>, to ensure maximum 
    /// compatibility to the widest range of email addresses. When 
    /// <see langword="true"/> any settings for <see cref="AllowAnyLetter"/>, 
    /// <see cref="AllowAnyDigit"/> and <see cref="AdditionalAllowedCharacters"/> 
    /// are ignored.
    /// </summary>
    public bool AllowAnyCharacter { get; set; } = true;

    /// <summary>
    /// Allows an email to contain any character classed as a unicode letter as 
    /// determined by <see cref="Char.IsLetter"/>. This setting is ignored when
    /// <see cref="AllowAnyCharacter"/> is set to <see langword="true"/>, which is the 
    /// default behavior.
    /// </summary>
    public bool AllowAnyLetter { get; set; } = true;

    /// <summary>
    /// Allows an email to contain any character classed as a decimal digit as 
    /// determined by <see cref="Char.IsDigit"/> i.e 0-9. This setting is ignored when
    /// <see cref="AllowAnyCharacter"/> is set to <see langword="true"/>, which is the 
    /// default behavior.
    /// </summary>
    public bool AllowAnyDigit { get; set; } = true;

    /// <summary>
    /// Allows any of the specified characters in addition to the letters or digit
    /// characters permitted by the <see cref="AllowAnyLetter"/> and <see cref="AllowAnyDigit"/>
    /// settings. This setting is ignored when <see cref="AllowAnyCharacter"/> is set to
    /// <see langword="true"/>, which is the default behavior. The @ symbol is always permitted.
    /// <para>
    /// The default settings specifies the range of special characters permitted in unquoted
    /// email addresses, excluding comment parentheses "()", and the square brackets "[]" that are 
    /// used to denote an IP address instead of a domain i.e "!#$%&'*+-/=?^_`{|}~.@". When enabling
    /// or altering these settings please be aware of the full extent of acceptable email formats,
    /// see https://en.wikipedia.org/wiki/Email_address#Syntax for an overview.
    /// </para>
    /// </summary>
    public string AdditionalAllowedCharacters { get; set; } = "!#$%&'*+-/=?^_`{|}~.";

    /// <summary>
    /// The minimum length of an email address. Defaults to 3. Must be between 3 and 
    /// 150 characters.
    /// </summary>
    public int MinLength { get; set; } = 3;

    /// <summary>
    /// The maximum length of an email address. Defaults to 150 characters and must be between 3 
    /// and 150 characters.
    /// </summary>
    /// <remarks>
    /// Technically email addresses can be longer than 150 characters (64 bytes for local + 256 bytes 
    /// for domain), but an arbitary limit of 150 is in place to limit index sizes.
    /// </remarks>
    [Range(3, UsernameOptions.MAX_LENGTH_BOUNDARY)]
    public int MaxLength { get; set; } = UsernameOptions.MAX_LENGTH_BOUNDARY;

    /// <summary>
    /// <para>
    /// Set this to <see langword="true"/> to ensure that an email cannot
    /// be allocated to more than one user per user area. Note that if
    /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> is set to <see langword="true"/>
    /// then this setting is ignored because usernames have to be unique.
    /// </para>
    /// <para>
    /// This defaults to <see langword="false"/> because a uniqueness check during
    /// registration can expose whether an email is registered or not, which may be
    /// sensitive information depending on the nature of the application.
    /// </para>
    /// </summary>
    public bool RequireUnique { get; set; }

    /// <summary>
    /// Copies the options to a new instance, which can be modified
    /// without altering the base settings. This is used for user area
    /// specific configuration.
    /// </summary>
    public EmailAddressOptions Clone()
    {
        return new EmailAddressOptions()
        {
            AllowAnyLetter = AllowAnyLetter,
            AllowAnyDigit = AllowAnyDigit,
            AdditionalAllowedCharacters = AdditionalAllowedCharacters,
            RequireUnique = RequireUnique,
            AllowAnyCharacter = AllowAnyCharacter,
            MinLength = MinLength,
            MaxLength = MaxLength
        };
    }
}
