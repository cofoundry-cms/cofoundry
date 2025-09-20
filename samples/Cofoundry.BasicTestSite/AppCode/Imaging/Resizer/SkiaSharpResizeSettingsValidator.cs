using Cofoundry.Domain;

namespace Cofoundry.Plugins.Imaging.SkiaSharp;

/// <summary>
/// Default implementation of <see cref="ISkiaSharpResizeSettingsValidator"/>.
/// </summary>
public class SkiaSharpResizeSettingsValidator : ISkiaSharpResizeSettingsValidator
{
    private readonly IImageResizeSettingsValidator _imageResizeSettingsValidator;

    public SkiaSharpResizeSettingsValidator(
        IImageResizeSettingsValidator imageResizeSettingsValidator
        )
    {
        _imageResizeSettingsValidator = imageResizeSettingsValidator;
    }

    /// <inheritdoc/>
    public void Validate(IImageResizeSettings settings, IImageAssetRenderable asset)
    {
        _imageResizeSettingsValidator.Validate(settings, asset);
    }
}
