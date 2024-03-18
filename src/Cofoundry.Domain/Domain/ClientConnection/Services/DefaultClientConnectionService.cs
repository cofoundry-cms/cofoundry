﻿namespace Cofoundry.Domain.Internal;

/// <summary>
/// The default IClientConnectionService implementation does not 
/// return any client data so that by default non-web scenarios 
/// are supported.
/// </summary>
public class DefaultClientConnectionService : IClientConnectionService
{
    /// <summary>
    /// Gets an object that represents the current client
    /// connected to the application in the current request.
    /// </summary>
    public ClientConnectionInfo GetConnectionInfo()
    {
        var info = new ClientConnectionInfo()
        {
            IPAddress = IPAddressConstants.Default
        };

        return info;
    }
}

