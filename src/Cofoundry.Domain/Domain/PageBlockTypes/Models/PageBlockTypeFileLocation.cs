namespace Cofoundry.Domain;

/// <summary>
/// Represents the location of a view file for
/// a page block type.
/// </summary>
public class PageBlockTypeFileLocation
{
    /// <summary>
    /// The file name (without extension) which is used as the unique 
    /// identifier for this page block type.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// The virtual path to the view file.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Locations of any alternative template view files, indexed by FileName.
    /// </summary>
    public IReadOnlyDictionary<string, PageBlockTypeTemplateFileLocation> Templates { get; set; } = ImmutableDictionary<string, PageBlockTypeTemplateFileLocation>.Empty;
}
