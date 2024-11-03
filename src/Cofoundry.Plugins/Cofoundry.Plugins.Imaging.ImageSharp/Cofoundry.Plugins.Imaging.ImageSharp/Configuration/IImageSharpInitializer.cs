using SixLabors.ImageSharp;

namespace Cofoundry.Plugins.Imaging.ImageSharp;

/// <summary>
/// Run at startup to initialize image sharp configuration. You can
/// implement your own initializer class to override the default 
/// using the DI system.
/// </summary>
public interface IImageSharpInitializer
{
    /// <summary>
    /// Initialized the ImageSharp configuration. This is run once
    /// at startup.
    /// </summary>
    void Initialize(Configuration configuration);
}
