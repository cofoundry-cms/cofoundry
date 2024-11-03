using SkiaSharp;

namespace Cofoundry.Plugins.Imaging.SkiaSharp;

/// <summary>
/// Represents all the dimensions and coordinates of a resize operation.
/// This is separated from the image resizing process to make it easier
/// to test and reason with.
/// </summary>
public class ResizeSpecification
{
    /// <summary>
    /// Indicates if the resized image needs to support transparency.
    /// </summary>
    public bool UsesTransparency { get; set; }

    /// <summary>
    /// Optional background color that should be used when padding an 
    /// image, or as a background to a transparent image. If empty then
    /// the default will be used: transparent or white for formats that
    /// do not suppport transparency.
    /// </summary>
    public SKColor? BackgroundColor { get; set; }

    /// <summary>
    /// The width of the resized canvas to paint the image on to.
    /// </summary>
    public int CanvasWidth { get; set; }

    /// <summary>
    /// The height of the resized canvas to paint the image on to.
    /// </summary>
    public int CanvasHeight { get; set; }

    /// <summary>
    /// The width of the resized but uncropped image, which may extend beyond the canvas.
    /// </summary>
    public int UncroppedImageWidth { get; set; }

    /// <summary>
    /// The height of the resized but uncropped image, which may extend beyond the canvas.
    /// </summary>
    public int UncroppedImageHeight { get; set; }

    /// <summary>
    /// The visible width of the resized and cropped image. This exludes any cropped
    /// width that extends beyond the canvas.
    /// </summary>
    public int VisibleImageWidth { get; set; }

    /// <summary>
    /// The visible height of the resized and cropped image. This exludes any cropped
    /// height that extends beyond the canvas.
    /// </summary>
    public int VisibleImageHeight { get; set; }

    /// <summary>
    /// The coordinates of the top-left position to start drawing the image, which may
    /// be off-canvas.
    /// </summary>
    public SKPoint AnchorAt { get; set; }

    /// <summary>
    /// The EXIF rotation value that is used to auto-rotate the image.
    /// </summary>
    public SKEncodedOrigin Origin { get; internal set; }

    /// <summary>
    /// True if the image is smaller than the canvas.
    /// </summary>
    /// <returns></returns>
    public bool RequiresPadding()
    {
        return UncroppedImageHeight < CanvasHeight || UncroppedImageWidth < CanvasWidth;
    }
}
