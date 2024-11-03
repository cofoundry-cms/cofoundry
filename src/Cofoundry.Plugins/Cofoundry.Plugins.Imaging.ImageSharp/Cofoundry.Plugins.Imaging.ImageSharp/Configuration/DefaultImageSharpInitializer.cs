using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;

namespace Cofoundry.Plugins.Imaging.ImageSharp;

/// <summary>
/// Default implementation of <see cref="IImageSharpInitializer"/>.
/// </summary>
public class DefaultImageSharpInitializer : IImageSharpInitializer
{
    private readonly ImageSharpSettings _imageSharpSettings;

    public DefaultImageSharpInitializer(
        ImageSharpSettings imageSharpSettings
        )
    {
        _imageSharpSettings = imageSharpSettings;
    }

    /// <inheritdoc/>
    public void Initialize(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetEncoder(JpegFormat.Instance, new JpegEncoder()
        {
            Quality = _imageSharpSettings.JpegQuality,
            SkipMetadata = _imageSharpSettings.IgnoreMetadata
        });

        configuration.ImageFormatsManager.SetEncoder(GifFormat.Instance, new GifEncoder()
        {
            SkipMetadata = _imageSharpSettings.IgnoreMetadata
        });

        configuration.ImageFormatsManager.SetEncoder(PngFormat.Instance, new PngEncoder()
        {
            SkipMetadata = _imageSharpSettings.IgnoreMetadata,
        });

        configuration.ImageFormatsManager.SetEncoder(WebpFormat.Instance, new WebpEncoder()
        {
            SkipMetadata = _imageSharpSettings.IgnoreMetadata,
        });
    }
}
