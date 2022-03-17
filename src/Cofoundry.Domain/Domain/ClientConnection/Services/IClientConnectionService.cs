namespace Cofoundry.Domain;

/// <summary>
/// Service for retreiving connection information about a client connected 
/// to the application e.g. IPAddress and the UserAgent string.
/// </summary>
public interface IClientConnectionService
{
    /// <summary>
    /// Gets an object that represents the current client
    /// connected to the application in the current request.
    /// </summary>
    ClientConnectionInfo GetConnectionInfo();
}
