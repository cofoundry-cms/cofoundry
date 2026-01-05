namespace Cofoundry.Domain.Data;

/// <summary>
/// Page block data for a specific page version. Each page block appears 
/// in a template region. The data and rendering of each block is controlled 
/// by the page block type assigned to it.
/// </summary>
public class PageVersionBlock : ICreateAuditable, IEntityVersionPageBlock
{
    /// <summary>
    /// Auto-incrementing primary key of the record in the database.
    /// </summary>
    public int PageVersionBlockId { get; set; }

    /// <summary>
    /// Id of the page version that this instance is
    /// parented to.
    /// </summary>
    public int PageVersionId { get; set; }

    /// <summary>
    /// The page version that this instance is
    /// parented to.
    /// </summary>
    public PageVersion PageVersion
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageVersionBlock>(nameof(PageVersion));
        set;
    }

    /// <inheritdoc/>
    public int PageTemplateRegionId { get; set; }

    /// <inheritdoc/>
    public PageTemplateRegion PageTemplateRegion
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageVersionBlock>(nameof(PageTemplateRegion));
        set;
    }

    /// <inheritdoc/>
    public int PageBlockTypeId { get; set; }

    /// <inheritdoc/>
    public PageBlockType PageBlockType
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageVersionBlock>(nameof(PageBlockType));
        set;
    }

    /// <inheritdoc/>
    public string SerializedData { get; set; } = string.Empty;

    /// <summary>
    /// In regions that support multiple block types this indicates
    /// the ordering of modules in the region.
    /// </summary>
    public int Ordering { get; set; }

    /// <inheritdoc/>
    public int? PageBlockTypeTemplateId { get; set; }

    /// <inheritdoc/>
    public PageBlockTypeTemplate? PageBlockTypeTemplate { get; set; }

    /// <inheritdoc/>
    public DateTime CreateDate { get; set; }

    /// <inheritdoc/>
    public int CreatorId { get; set; }

    /// <inheritdoc/>
    public User Creator
    {
        get => field ?? throw NavigationPropertyNotInitializedException.Create<PageVersionBlock>(nameof(Creator));
        set;
    }

    /// <summary>
    /// Date that the block was last updated.
    /// </summary>
    public DateTime UpdateDate { get; set; }

    /// <inheritdoc/>
    public int GetVersionBlockId()
    {
        return PageVersionBlockId;
    }
}
