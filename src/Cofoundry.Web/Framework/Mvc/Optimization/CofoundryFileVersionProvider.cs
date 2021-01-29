// Adapted from https://github.com/aspnet/AspNetCore/blob/master/src/Mvc/src/Microsoft.AspNetCore.Mvc.Razor/Infrastructure/DefaultFileVersionProvider.cs
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See https://github.com/aspnet/AspNetCore/blob/master/LICENSE.txt for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Cofoundry.Web
{
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
    /// Adapted from https://github.com/aspnet/AspNetCore/blob/master/src/Mvc/src/Microsoft.AspNetCore.Mvc.Razor/Infrastructure/DefaultFileVersionProvider.cs
    /// </para>
    /// </remarks>
    public class CofoundryFileVersionProvider : IFileVersionProvider
    {
        private const string VersionKey = "v";
        private static readonly char[] QueryStringAndFragmentTokens = new[] { '?', '#' };

        private readonly IFileProvider _fileProvider;
        private readonly IMemoryCache _cache;

        public CofoundryFileVersionProvider(
            IFileProvider fileProvider,
            IMemoryCache memoryCache
            )
        {
            if (fileProvider == null)
            {
                throw new ArgumentNullException(nameof(fileProvider));
            }

            if (memoryCache == null)
            {
                throw new ArgumentNullException(nameof(memoryCache));
            }

            _fileProvider = fileProvider;
            _cache = memoryCache;
        }

        /// <remarks>
        /// Only private member variables modified from DefaultFileProvider
        /// </remarks>
        public string AddFileVersionToPath(PathString requestPathBase, string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var resolvedPath = path;

            var queryStringOrFragmentStartIndex = path.IndexOfAny(QueryStringAndFragmentTokens);
            if (queryStringOrFragmentStartIndex != -1)
            {
                resolvedPath = path.Substring(0, queryStringOrFragmentStartIndex);
            }

            if (Uri.TryCreate(resolvedPath, UriKind.Absolute, out var uri) && !uri.IsFile)
            {
                // Don't append version if the path is absolute.
                return path;
            }

            if (_cache.TryGetValue(path, out string value))
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
            value = _cache.Set(path, value, cacheEntryOptions);
            return value;
        }

        private static string GetHashForFile(IFileInfo fileInfo)
        {
            // Removed because the wrapped exception does not apply to .NET Core
            //using (var sha256 = CryptographyAlgorithms.CreateSHA256())
            using (var sha256 = SHA256.Create())
            {
                using (var readStream = fileInfo.CreateReadStream())
                {
                    var hash = sha256.ComputeHash(readStream);
                    return WebEncoders.Base64UrlEncode(hash);
                }
            }
        }
    }
}
