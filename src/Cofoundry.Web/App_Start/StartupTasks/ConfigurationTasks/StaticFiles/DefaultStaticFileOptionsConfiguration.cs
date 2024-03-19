﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

namespace Cofoundry.Web;

/// <summary>
/// The default static file configuration simply sets caching headers using rules that
/// should work for most configurations.
/// </summary>
public class DefaultStaticFileOptionsConfiguration : IStaticFileOptionsConfiguration
{
    private readonly StaticFilesSettings _staticFilesSettings;

    public DefaultStaticFileOptionsConfiguration(StaticFilesSettings staticFilesSettings)
    {
        _staticFilesSettings = staticFilesSettings;
    }

    public void Configure(StaticFileOptions options)
    {
        options.OnPrepareResponse = OnPrepareResponse;
    }

    private void OnPrepareResponse(StaticFileResponseContext context)
    {
        bool setCachingHeaders;

        switch (_staticFilesSettings.CacheMode)
        {
            case StaticFileCacheMode.None:
                return;
            case StaticFileCacheMode.All:
                setCachingHeaders = true;
                break;
            case StaticFileCacheMode.OnlyVersionedFiles:
                // only cache resources that use the asp-append-version parameter convention
                setCachingHeaders = !string.IsNullOrEmpty(context.Context.Request.Query["v"]);
                break;
            default:
                throw new InvalidOperationException("StaticFileCacheMode not regonised: " + _staticFilesSettings.CacheMode);
        }

        if (!setCachingHeaders)
        {
            return;
        }

        // cache for 1 year
        context.Context.Response.Headers[HeaderNames.CacheControl] = new[] { "public,max-age=" + _staticFilesSettings.MaxAge };
    }
}
