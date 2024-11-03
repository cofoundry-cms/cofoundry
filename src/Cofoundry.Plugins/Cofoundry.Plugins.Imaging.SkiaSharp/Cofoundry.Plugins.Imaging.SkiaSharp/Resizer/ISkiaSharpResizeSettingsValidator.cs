using Cofoundry.Domain;

namespace Cofoundry.Plugins.Imaging.SkiaSharp;

/// <summary>
/// Used to validate the resize settings sent from the client prior
/// to resizing the image.
/// </summary>
public interface ISkiaSharpResizeSettingsValidator
{
    /// <summary>
    /// Validates that the image asset can be resized using the settings
    /// supplied by the client.
    /// </summary>
    /// <param name="settings">Settings to validate.</param>
    /// <param name="asset">Asset attempting to be resized. Cannot be null.</param>
    void Validate(IImageResizeSettings settings, IImageAssetRenderable asset);
}
