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

    private PageVersion? _pageVersion;
    /// <summary>
    /// The page version that this instance is
    /// parented to.
    /// </summary>
    public PageVersion PageVersion
    {
        get => _pageVersion ?? throw NavigationPropertyNotInitializedException.Create<PageVersionBlock>(nameof(PageVersion));
        set => _pageVersion = value;
    }

    /// <inheritdoc/>
    public int PageTemplateRegionId { get; set; }

    private PageTemplateRegion? _pageTemplateRegion;
    /// <inheritdoc/>
    public PageTemplateRegion PageTemplateRegion
    {
        get => _pageTemplateRegion ?? throw NavigationPropertyNotInitializedException.Create<PageVersionBlock>(nameof(PageTemplateRegion));
        set => _pageTemplateRegion = value;
    }

    /// <inheritdoc/>
    public int PageBlockTypeId { get; set; }

    private PageBlockType? _pageBlockType;
    /// <inheritdoc/>
    public PageBlockType PageBlockType
    {
        get => _pageBlockType ?? throw NavigationPropertyNotInitializedException.Create<PageVersionBlock>(nameof(PageBlockType));
        set => _pageBlockType = value;
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

    private User? _creator;
    /// <inheritdoc/>
    public User Creator
    {
        get => _creator ?? throw NavigationPropertyNotInitializedException.Create<PageVersionBlock>(nameof(Creator));
        set => _creator = value;
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
