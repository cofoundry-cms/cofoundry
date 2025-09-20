namespace Cofoundry.Plugins.Imaging.SkiaSharp;

/// <summary>
/// Used to indicate the bahaviour that should be applied to gifs as
/// they are unsupported by SkiaSharp
/// </summary>
public enum GifResizeBehaviour
{
    /// <summary>
    /// Save single frame gifs as PNGs, but leave animated gifs untouched
    /// and do not resize them.
    /// </summary>
    Auto,

    /// <summary>
    /// Dont resize, output the original image instead.
    /// </summary>
    NoResize,

    /// <summary>
    /// Re-encoded all gifs to pngs when they are uploaded.
    /// </summary>
    ResizeAsAlternative
}
