using Microsoft.AspNetCore.Http;

namespace Cofoundry.Domain;

/// <summary>
/// Simple helper for formatting a url that includes an authenticated task 
/// token using a standard set of parameters.
/// </summary>
public interface IAuthorizedTaskTokenUrlHelper
{
    /// <summary>
    /// Formats an authenticated task token url. Url is in the format "{baseUrl}?t={token}"
    /// </summary>
    /// <param name="baseUrl">
    /// The relative base path used to construct the url 
    /// e.g. "/auth/forgot-password".
    /// </param>
    /// <param name="token">The token used to identify and authenticate the task.</param>
    /// <returns>The formatted url.</returns>
    string MakeUrl(
        string baseUrl,
        string token
        );

    /// <summary>
    /// Parses an authenticated task token from a querystring
    /// collection. The collection should come from a request with
    /// a url created using <see cref="IAuthorizedTaskTokenUrlHelper.MakeUrl"/>.
    /// </summary>
    /// <param name="queryCollection">
    /// Querystring parameters, which can be accessed in a controller
    /// using "Request.Query" or via "HttpContext.Request.Query".
    /// </param>
    /// <returns>
    /// Parsed token value, which may be <see langword="null"/> if the token
    /// parameter could not be found.
    /// </returns>
    string ParseTokenFromQuery(IQueryCollection queryCollection);
}
