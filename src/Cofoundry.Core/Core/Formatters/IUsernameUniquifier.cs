namespace Cofoundry.Core;

/// <summary>
/// Used to format usernames into a consistent format that can be
/// used for comparing usernames e.g. as a unique value to prevent duplication
/// and during sign in to lookup a username. The default implementation 
/// lowercases the username.
/// </summary>
public interface IUsernameUniquifier
{
    /// <summary>
    /// <para>
    /// Formats the specified username into a consistent format that can be
    /// used for comparing usernames e.g. as a unique value to prevent duplication
    /// and during sign in to lookup a username. The default implementation 
    /// lowercases the username. If the username is null or empty then
    /// <see langword="null"/> is returned.
    /// </para>
    /// <para>
    /// As an example, if the user types their username as " L.Balfour" then under
    /// default rules the username would normalize via <see cref="IUsernameNormalizer"/> 
    /// as "L.Balfour" and uniquify as "l.balfour". 
    /// </para>
    /// </summary>
    /// <param name="username">
    /// The username string to format. If the value is <see langword="null"/> 
    /// then <see langword="null"/> is returned.
    /// </param>
    string Uniquify(string username);
}
