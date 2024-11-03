using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Plugins.Imaging.ImageSharp;

/// <summary>
/// An attempt to cover some of the most popular image sharp configuration
/// settings. Anything more compilated can be acheived by setting the ImageSharp
/// configuration manually by implementing your own IImageSharpInitializer or 
/// by configuring SixLabors.ImageSharp.Configuration.Default. 
/// </summary>
public class ImageSharpSettings : PluginConfigurationSettingsBase
{
    public ImageSharpSettings()
    {
        IgnoreMetadata = true;
    }

    /// <summary>
    /// Jpeg quality setting out of 100. Defaults to 85.
    /// </summary>
    [Range(0, 100)]
    public int JpegQuality { get; set; } = 85;

    /// <summary>
    /// Indicates whether the metadata should be ignored when the
    /// image is being encoded.
    /// </summary>
    public bool IgnoreMetadata { get; set; }
}
