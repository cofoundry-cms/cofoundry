using Cofoundry.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Plugins.Imaging.SkiaSharp;

/// <summary>
/// Basic settings for image resizing behavior using SkiaSharp.
/// </summary>
public class SkiaSharpSettings : PluginConfigurationSettingsBase
{
    public SkiaSharpSettings()
    {
        JpegQuality = 85;
    }

    /// <summary>
    /// Jpeg quality setting out of 100. Defaults to 85.
    /// </summary>
    [Range(0, 100)]
    public int JpegQuality { get; set; }

    /// <summary>
    /// Gifs can be saved but not resized. Use this to customize the 
    /// fallback behaviour.
    /// </summary>
    public GifResizeBehaviour GifResizeBehaviour { get; set; }

    /// <summary>
    /// Disables reading and writing of caching of resized image files.
    /// Useful for debugging, but note that cache headers are still set and
    /// files may be cached upstream.
    /// </summary>
    public bool DisableFileCache { get; set; }
}
