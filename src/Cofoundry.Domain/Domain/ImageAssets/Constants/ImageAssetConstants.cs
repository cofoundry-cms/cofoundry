namespace Cofoundry.Domain;

public static class ImageAssetConstants
{
    /// <summary>
    /// The name of the file container (folder) to store image asset
    /// files in.
    /// </summary>
    public const string FileContainerName = "Images";

    /// <summary>
    /// Svg file extension, lowercase without the leading dot "svg".
    /// </summary>
    public const string SvgFileExtension = "svg";

    /// <summary>
    /// A lookup of image types that are allowed to be uploaded in the admin
    /// panel.
    /// </summary>
    public static readonly Dictionary<string, string> PermittedImageTypes = new()
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".bmp", "image/bmp" },
        { ".tif", "image/tif" },
        { ".tiff", "image/tif" },
        { ".webp", "image/webp" },
        { ".svg", "image/svg+xml" }
    };
}
