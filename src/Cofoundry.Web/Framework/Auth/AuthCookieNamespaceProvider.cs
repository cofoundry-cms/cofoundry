using Microsoft.Extensions.Hosting;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IAuthCookieNamespaceProvider"/>.
/// </summary>
public class AuthCookieNamespaceProvider : IAuthCookieNamespaceProvider
{
    private readonly IHostEnvironment _hostingEnvironment;
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

    public AuthCookieNamespaceProvider(
        IHostEnvironment hostingEnvironment,
        IUserAreaDefinitionRepository userAreaDefinitionRepository
        )
    {
        _hostingEnvironment = hostingEnvironment;
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
    }

    /// <inheritdoc/>
    public string GetNamespace(string userAreaCode)
    {
        var options = _userAreaDefinitionRepository.GetOptionsByCode(userAreaCode).Cookies;

        if (!string.IsNullOrWhiteSpace(options.Namespace))
        {
            return options.Namespace;
        }

        // Try and build a short and somewhat unique name using the 
        // application name, which should suffice for most scenarios. 
        var appName = _hostingEnvironment.ApplicationName;

        var reasonablyUniqueName = appName
            .Take(3)
            .Union(appName.Reverse())
            .Take(6);

        return "CFA_" + string.Concat(reasonablyUniqueName);
    }
}
