namespace Cofoundry.Core;

/// <summary>
/// <para>
/// Used to normalize usernames into a consistent format. This normalizer
/// is used before saving the main username field . This field may be used for 
/// display and so it's rare that you'd want to change it dramatically, therefore the default 
/// implementation simply trims the input.
/// </para>
/// <para>
/// <see cref="IUsernameUniquifier"/> is used to format the username for comparisons and may 
/// use further formatting such as lowercasing, which is the default behavior.
/// </para>
/// </summary>
public interface IUsernameNormalizer
{
    /// <summary>
    /// <para>
    /// Normalizes the specified username into a consistent format. This normalizer
    /// is used before saving the main username field. This field may be used for 
    /// display and so it's rare that you'd want to change it dramatically, therefore the default 
    /// implementation simply trims the input.
    /// </para>
    /// <para>
    /// <see cref="IUsernameUniquifier"/> is used to format the username for comparisons and may 
    /// use further formatting such as lowercasing, which is the default behavior.
    /// </para>
    /// </summary>
    /// <param name="username">The username string to format.</param>
    string Normalize(string username);
}
