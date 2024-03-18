﻿namespace Cofoundry.Core;

/// <summary>
/// IPathResolver for resolving virtual paths relative to the executing assembly.
/// This is mainly intended for resolving paths in console apps.
/// </summary>
public class FileSystemPathResolver : IPathResolver
{
    public string MapPath(string? path)
    {
        var basePath = AppContext.BaseDirectory;

        if (string.IsNullOrWhiteSpace(path)) return basePath;

        path = path.TrimStart(['~', '/']);

        return Path.Combine(basePath, path);
    }
}
