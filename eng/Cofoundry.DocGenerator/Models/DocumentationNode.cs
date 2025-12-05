namespace Cofoundry.DocGenerator;

public class DocumentationNode
{
    /// <summary>
    /// Display title of the directory or route. Taken from
    /// the directory or file name.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The url path for this node. This should
    /// always have a value.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Path to the markdown file to render. Can be null for
    /// directories or redirects where no file is applicable. For
    /// directories that have a custom index file, this will point 
    /// to that index file.
    /// </summary>
    public string? DocumentFilePath { get; set; }

    /// <summary>
    /// Optional redirect value
    /// </summary>
    public string? RedirectTo { get; set; }

    /// <summary>
    /// Date the file or directory was last written to.
    /// </summary>
    public DateTime UpdateDate { get; set; }

    public IReadOnlyCollection<DocumentationNode> Children { get; set; } = [];
}
