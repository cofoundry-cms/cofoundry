using SkiaSharp;

namespace Cofoundry.Plugins.Imaging.SkiaSharp;

/// <summary>
/// Default implementation of <see cref="ISkiaSharpImageResizer"/>.
/// </summary>
public class SkiaSharpImageResizer : ISkiaSharpImageResizer
{
    /// <inheritdoc/>
    public SKImage Resize(SKBitmap sourceImage, ResizeSpecification resizeSpecification)
    {
        var canvasSpecification = sourceImage.Info.WithSize(resizeSpecification.CanvasWidth, resizeSpecification.CanvasHeight);

        // Some formats that support transparency aren't always saved with transparency enabled, so we
        // need to enable this to avoid a black background when padding
        if (resizeSpecification.UsesTransparency && canvasSpecification.AlphaType != SKAlphaType.Premul)
        {
            canvasSpecification = canvasSpecification.WithAlphaType(SKAlphaType.Premul);
        }

        using (var surface = SKSurface.Create(canvasSpecification))
        {
            var canvas = surface.Canvas;

            if ((resizeSpecification.RequiresPadding() || resizeSpecification.UsesTransparency)
                && resizeSpecification.BackgroundColor.HasValue)
            {
                canvas.Clear(resizeSpecification.BackgroundColor.Value);
            }

            var newSize = new SKSizeI(resizeSpecification.UncroppedImageWidth, resizeSpecification.UncroppedImageHeight);
            using (var resizedBitmap = sourceImage.Resize(newSize, SKFilterQuality.High))
            using (var resizedImage = SKImage.FromBitmap(resizedBitmap))
            {
                surface.Canvas.DrawImage(resizedImage, resizeSpecification.AnchorAt);
            }
            surface.Canvas.Flush();

            return surface.Snapshot();
        }
    }
}
