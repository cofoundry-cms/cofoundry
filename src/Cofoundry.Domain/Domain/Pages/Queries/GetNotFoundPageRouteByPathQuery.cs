﻿namespace Cofoundry.Domain;

/// <summary>
/// Attempts to find the most relevant 'Not Found' page route by searching
/// for a 'Not Found' page up the directory tree of a specific path.
/// </summary>
public class GetNotFoundPageRouteByPathQuery : IQuery<PageRoute?>
{
    /// <summary>
    /// Id of the locale to look for when searching for a non-found page.
    /// </summary>
    public int? LocaleId { get; set; }

    /// <summary>
    /// Path of the page that could not be found.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether to include unpublished page routes in the result.
    /// </summary>
    public bool IncludeUnpublished { get; set; }
}
