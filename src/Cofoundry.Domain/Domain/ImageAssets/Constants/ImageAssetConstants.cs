﻿namespace Cofoundry.Domain;

public static class ImageAssetConstants
{
    public static readonly string FileContainerName = "Images";
    public static readonly Dictionary<string, string> PermittedImageTypes = new()
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".bmp", "image/bmp" },
        { ".tif", "image/tif" },
        { ".tiff", "image/tif" }
    };
}
