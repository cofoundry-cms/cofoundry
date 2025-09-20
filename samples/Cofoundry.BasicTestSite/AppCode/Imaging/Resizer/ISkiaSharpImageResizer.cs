using SkiaSharp;

namespace Cofoundry.Plugins.Imaging.SkiaSharp;

/// <summary>
/// Resizes image pixel data, based on the specification, but does
/// not dave the file.
/// </summary>
public interface ISkiaSharpImageResizer
{
    /// <summary>
    /// 
    /// <summary>
    /// Resizes image pixel data, based on the specification, but does
    /// not dave the file. The caller should manager the disposal of
    /// the resulting SKImage.
    /// </summary>
    /// </summary>
    /// <param name="sourceImage">Source image data.</param>
    /// <param name="resizeSpecification">Specification detailing how the image should be resized.</param>
    SKImage Resize(SKBitmap sourceImage, ResizeSpecification resizeSpecification);
}
