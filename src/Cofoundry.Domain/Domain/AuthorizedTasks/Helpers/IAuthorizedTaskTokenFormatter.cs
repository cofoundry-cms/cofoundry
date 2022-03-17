namespace Cofoundry.Domain;

/// <summary>
/// Used to create a single token that identifies
/// and authorizes an AuthorizedTask.
/// </summary>
public interface IAuthorizedTaskTokenFormatter
{
    /// <summary>
    /// Formats a new token from the specified <paramref name="tokenParts"/>.
    /// </summary>
    /// <param name="tokenParts">
    /// The data to use to create the token, which must not be 
    /// <see langword="null"/> or contain empty data.
    /// </param>
    public string Format(AuthorizedTaskTokenParts tokenParts);

    /// <summary>
    /// Parses a token back to its constituent parts. If the token
    /// could not be parsed or if any parts are not present then
    /// <see langword="null"/> is returned.
    /// </summary>
    /// <param name="token">
    /// The token to parse. Can be <see langword="null"/> 
    /// which will return a <see langword="null"/> result.
    /// </param>
    public AuthorizedTaskTokenParts Parse(string token);
}
