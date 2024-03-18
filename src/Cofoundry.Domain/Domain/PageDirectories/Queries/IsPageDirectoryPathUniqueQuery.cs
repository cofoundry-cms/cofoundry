﻿namespace Cofoundry.Domain;

/// <summary>
/// Query to determine if a page directory UrlPath is unique
/// within its parent directory.
/// </summary>
public class IsPageDirectoryPathUniqueQuery : IQuery<bool>
{
    /// <summary>
    /// Optional Id of the page directory being updated so it can
    /// be removed from the check.
    /// </summary>
    public int? PageDirectoryId { get; set; }

    /// <summary>
    /// Id of the parent directory. The UrlPath will
    /// checked for uniqueness within this directory.
    /// </summary>
    public int? ParentPageDirectoryId { get; set; }

    /// <summary>
    /// The url path to check for uniqueness.
    /// </summary>
    public string UrlPath { get; set; } = string.Empty;
}
