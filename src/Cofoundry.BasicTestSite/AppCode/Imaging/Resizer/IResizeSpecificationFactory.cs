using Cofoundry.Domain;
using SkiaSharp;

namespace Cofoundry.Plugins.Imaging.SkiaSharp;

/// <summary>
/// Factory to produce a specification detailing the target dimensions and
/// cropping required in a resize operation.
/// </summary>
public interface IResizeSpecificationFactory
{
    /// <summary>
    /// Produces a specification detailing the target dimensions and
    /// cropping required in a resize operation.
    /// </summary>
    /// <param name="sourceCodec">
    /// The file header data from the source image file. This does not get disposed.
    /// </param>
    /// <param name="sourceImage">The source image pixel data. This does not get disposed.</param>
    /// <param name="resizeSettings">
    /// The resize settings that indicate the desired outcome and rules of the
    /// resize operation.
    /// </param>
    /// <returns></returns>
    ResizeSpecification Create(
        SKCodec sourceCodec,
        SKBitmap sourceImage,
        IImageResizeSettings resizeSettings
        );
}
