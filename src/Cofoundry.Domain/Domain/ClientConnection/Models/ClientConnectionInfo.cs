namespace Cofoundry.Domain;

/// <summary>
/// Contains connection information about a clients connection
/// to the application e.g. IPAddress and the UserAgent string.
/// </summary>
public class ClientConnectionInfo
{
    /// <summary>
    /// The IP Address of the client connected to the application.
    /// For non-network requests this value will be <see cref="IPAddressConstants.Default"/>.
    /// </summary>
    public required string IPAddress { get; set; }

    /// <summary>
    /// The connecting client's user agent string (if connecting via a web browser).
    /// </summary>
    public string? UserAgent { get; set; }
}
