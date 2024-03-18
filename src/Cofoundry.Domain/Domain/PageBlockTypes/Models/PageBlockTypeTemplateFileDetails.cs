﻿namespace Cofoundry.Domain;

/// <summary>
/// Represents the result of parsing a page block template view file
/// for information.
/// </summary>
public class PageBlockTypeTemplateFileDetails
{
    /// <summary>
    /// The name of the template view file without an extensions 
    /// e.g. 'H1', 'ReversedContent'. Must be unique to the block
    /// type.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// The display name for the template in the administration UI
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the template in the administration UI
    /// </summary>
    public string? Description { get; set; }
}
