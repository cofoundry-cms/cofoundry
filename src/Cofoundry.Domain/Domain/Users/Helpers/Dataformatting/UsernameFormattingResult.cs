namespace Cofoundry.Domain;

/// <summary>
/// The result of a call to <see cref="IUserDataFormatter.FormatUsername"/>
/// which contains all the username formats required to update the username 
/// on a <see cref="Data.User"/> record.
/// </summary>
public class UsernameFormattingResult
{
    /// <summary>
    /// The username that is used as the user identifier e.g. "JArnold" or "jarnold@example.com". 
    /// Depending on the user area settings this might be a normalized copy of the email address. 
    /// By default limited formatting is applied during normalization, whereas the <see cref="UniqueUsername"/> 
    /// field is formatted by a "uniquification" process which can be more involved, because that field used for 
    /// comparisons when logging in.
    /// </summary>
    public string NormalizedUsername { get; set; }

    /// <summary>
    /// A version of the username that is formatted to standardize casing and any other
    /// required formatting irregularities e.g. "jarnold" or "jarnold@example.com". This field 
    /// is used for uniqueness checks and user lookups.
    /// </summary>
    public string UniqueUsername { get; set; }
}
