using System.Net;

namespace Cofoundry.Domain;

public static class IImageResizeSettingsExtensions
{
    /// <summary>
    /// Determines if the settings indicate that an image asset should 
    /// be resized. If width and hight are not specified or the dimensions
    /// in the settings are the same as the image then false will be returned.
    /// </summary>
    /// <param name="settings">
    /// Settings instance to query.
    /// </param>
    /// <param name="asset">The asset to check. Cannot be null.</param>
    public static bool RequiresResizing(this IImageResizeSettings settings, IImageAssetRenderable asset)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(asset);

        if ((settings.Width < 1 && settings.Height < 1)
            || (settings.Width == asset.Width && settings.Height == asset.Height))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Indicates if the resize width has been specified i.e. is greater than 0.
    /// </summary>
    /// <param name="settings">
    /// Settings instance to query.
    /// </param>
    public static bool IsWidthSet(this IImageResizeSettings settings)
    {
        return settings.Width > 0;
    }

    /// <summary>
    /// Indicates if the resize height has been specified i.e. is greater than 0.
    /// </summary>
    /// <param name="settings">
    /// Settings instance to query.
    /// </param>
    public static bool IsHeightSet(this IImageResizeSettings settings)
    {
        return settings.Height > 0;
    }

    /// <summary>
    /// Creates a short string unique to the settings which can be used
    /// as a file name.
    /// </summary>
    /// <param name="settings">
    /// Settings instance to map parameters from.
    /// </param>
    public static string CreateCacheFileName(this IImageResizeSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var fileName = $"w{settings.Width}h{settings.Height}c{settings.Mode}s{settings.Scale}bg{settings.BackgroundColor}a{settings.Anchor}";
        fileName = WebUtility.UrlEncode(fileName);

        return fileName;
    }
}
