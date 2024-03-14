// Adapted from https://github.com/dotnet/aspnetcore/blob/main/src/Mvc/Mvc.Razor/src/Infrastructure/DefaultFileVersionProvider.cs
// Copyright (c) .NET Foundation and Contributors. All rights reserved.
// Licensed under the MIT License. See https://github.com/dotnet/aspnetcore/blob/main/LICENSE.txt for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using System.Security.Cryptography;

namespace Cofoundry.Web;

/// <summary>
/// Provides version hash for a specified file.
/// </summary>
/// <remarks>
/// <para>
/// Cofoundry implementation of IFileVersionProvider that isn't specific
/// to the web application web root and instead allows you to pass the
/// file provider. We use this to allow file versioning to run on any
/// embedded resources.
/// </para>
/// <para>
/// Adapted from https://github.com/dotnet/aspnetcore/blob/main/src/Mvc/Mvc.Razor/src/Infrastructure/DefaultFileVersionProvider.cs
/// </para>
/// </remarks>
public class CofoundryFileVersionProvider : IFileVersionProvider
{
    private const string VersionKey = "v";

    private readonly IFileProvider _fileProvider;
    private readonly IMemoryCache _cache;

    public CofoundryFileVersionProvider(
        IFileProvider fileProvider,
        IMemoryCache memoryCache
        )
    {
        ArgumentNullException.ThrowIfNull(fileProvider);
        ArgumentNullException.ThrowIfNull(memoryCache);

        _fileProvider = fileProvider;
        _cache = memoryCache;
    }

    /// <remarks>
    /// Only private member variables modified from DefaultFileProvider
    /// </remarks>
    public string AddFileVersionToPath(PathString requestPathBase, string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var resolvedPath = path;

        var queryStringOrFragmentStartIndex = path.AsSpan().IndexOfAny('?', '#');
        if (queryStringOrFragmentStartIndex != -1)
        {
            resolvedPath = path.Substring(0, queryStringOrFragmentStartIndex);
        }

        if (Uri.TryCreate(resolvedPath, UriKind.Absolute, out var uri) && !uri.IsFile)
        {
            // Don't append version if the path is absolute.
            return path;
        }

        if (_cache.TryGetValue(path, out string? value) && value is not null)
        {
            return value;
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions();
        cacheEntryOptions.AddExpirationToken(_fileProvider.Watch(resolvedPath));
        var fileInfo = _fileProvider.GetFileInfo(resolvedPath);

        if (!fileInfo.Exists &&
            requestPathBase.HasValue &&
            resolvedPath.StartsWith(requestPathBase.Value, StringComparison.OrdinalIgnoreCase))
        {
            var requestPathBaseRelativePath = resolvedPath.Substring(requestPathBase.Value.Length);
            cacheEntryOptions.AddExpirationToken(_fileProvider.Watch(requestPathBaseRelativePath));
            fileInfo = _fileProvider.GetFileInfo(requestPathBaseRelativePath);
        }

        if (fileInfo.Exists)
        {
            value = QueryHelpers.AddQueryString(path, VersionKey, GetHashForFile(fileInfo));
        }
        else
        {
            // if the file is not in the current server.
            value = path;
        }

        cacheEntryOptions.SetSize(value.Length * sizeof(char));
        _cache.Set(path, value, cacheEntryOptions);

        return value;
    }

    private static string GetHashForFile(IFileInfo fileInfo)
    {
        using var readStream = fileInfo.CreateReadStream();
        var hash = SHA256.HashData(readStream);

        return WebEncoders.Base64UrlEncode(hash);
    }
}
