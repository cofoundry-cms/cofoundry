using Cofoundry.Web;
using Microsoft.AspNetCore.Builder;

namespace Cofoundry.Plugins.Imaging.ImageSharp;

public class ImageSharpConfigurationStartupTask : IStartupConfigurationTask
{
    private readonly IImageSharpInitializer _imageSharpConfiguration;

    public ImageSharpConfigurationStartupTask(
        IImageSharpInitializer imageSharpConfiguration
        )
    {
        _imageSharpConfiguration = imageSharpConfiguration;
    }

    public int Ordering => (int)StartupTaskOrdering.Early;

    public void Configure(IApplicationBuilder app)
    {
        // Setup some initial config
        var imgConfig = SixLabors.ImageSharp.Configuration.Default;

        _imageSharpConfiguration.Initialize(imgConfig);
    }
}
