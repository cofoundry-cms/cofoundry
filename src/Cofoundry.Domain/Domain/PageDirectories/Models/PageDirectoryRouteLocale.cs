﻿namespace Cofoundry.Domain;

/// <remarks>
/// The db supports a page directory having different paths per
/// locale, but this isn't used at the admin UI level and will
/// likely be revised at a future date.
/// </remarks>
public class PageDirectoryRouteLocale
{
    public int LocaleId { get; set; }

    public string UrlPath { get; set; } = string.Empty;

    public string FullUrlPath { get; set; } = string.Empty;
}
