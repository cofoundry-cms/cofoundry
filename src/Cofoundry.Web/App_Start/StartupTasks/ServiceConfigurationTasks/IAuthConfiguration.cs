using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Web;

/// <summary>
/// Configures MVC authentication during the Cofoundry startup process. You can 
/// create your own implementation of this to customize to completely customize the 
/// authenitcation process, or use <see cref="DefaultAuthConfiguration"/> as a base
/// if you just want to modify the default implementation.
/// </summary>
public interface IAuthConfiguration
{
    /// <summary>
    /// Applies authentication configuration to the IMvcBuilder.
    /// </summary>
    void Configure(IMvcBuilder mvcBuilder);
}
