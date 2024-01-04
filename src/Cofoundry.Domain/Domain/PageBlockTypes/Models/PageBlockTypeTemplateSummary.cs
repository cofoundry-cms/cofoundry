namespace Cofoundry.Domain;

/// <summary>
/// A block can optionally have display templates associated with it, 
/// which will give the user a choice about how the data is rendered out
/// e.g. 'Wide', 'Headline', 'Large', 'Reversed'. If no template is set then 
/// the default view is used for rendering.
/// </summary>
public class PageBlockTypeTemplateSummary
{
    /// <summary>
    /// Database id of the block type template record.
    /// </summary>
    public int PageBlockTypeTemplateId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// A placeholder value to use for not-nullable values that you
    /// know will be initialized in later code. This value should not
    /// be used in data post-initialization.
    /// </summary>
    public static readonly PageBlockTypeTemplateSummary Uninitialized = new()
    {
        PageBlockTypeTemplateId = int.MinValue
    };
}
