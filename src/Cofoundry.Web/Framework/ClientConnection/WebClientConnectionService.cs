﻿using Microsoft.AspNetCore.Http;

namespace Cofoundry.Web.Internal;

/// <summary>
/// Service for retreiving connection information about a client connected 
/// to the application e.g. IPAddress and the UserAgent string.
/// </summary>
public class WebClientConnectionService : IClientConnectionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WebClientConnectionService(
        IHttpContextAccessor httpContextAccessor
        )
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets an object that represents the current client
    /// connected to the application in the current request.
    /// </summary>
    public ClientConnectionInfo GetConnectionInfo()
    {
        var context = _httpContextAccessor.HttpContext;

        var info = new ClientConnectionInfo()
        {
            IPAddress = context?.Connection?.RemoteIpAddress?.ToString() ?? IPAddressConstants.Default,
            UserAgent = context?.Request?.Headers?.GetOrDefault("User-Agent")
        };

        return info;
    }
}

