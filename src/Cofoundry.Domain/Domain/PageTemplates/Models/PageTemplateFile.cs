﻿namespace Cofoundry.Domain;

/// <summary>
/// Information the location of a physical (or virtual) template file
/// </summary>
public class PageTemplateFile
{
    /// <summary>
    /// File name excluding extension and any leading underscores
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Full virtual path to the view file including the filename. This will
    /// be unique.
    /// </summary>
    public string VirtualPath { get; set; } = string.Empty;
}
