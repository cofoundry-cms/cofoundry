using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web;

/// <summary>
/// Implement this interface to extend the MvcOptions configuration in a modular 
/// fashion. Implementations can make use of dependency injection, however this is
/// built using a temporary service collection that will be disposed of after 
/// configuration is complete.
/// </summary>
public interface IMvcOptionsConfiguration
{
    /// <summary>
    /// Performs additional option configuration. 
    /// </summary>
    /// <param name="options">The options to perform configuration on.</param>
    void Configure(MvcOptions options);
}
