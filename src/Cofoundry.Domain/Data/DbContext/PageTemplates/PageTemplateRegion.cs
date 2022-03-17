namespace Cofoundry.Domain.Data;

/// <summary>
/// Each PageTemplate can have zero or more regions which are defined in the 
/// template file using the CofoundryTemplate helper, 
/// e.g. @Cofoundry.Template.Region("MyRegionName"). These regions represent
/// areas where page blocks can be placed (i.e. insert content).
/// </summary>
public class PageTemplateRegion
{
    /// <summary>
    /// The database id of the region.
    /// </summary>
    public int PageTemplateRegionId { get; set; }

    /// <summary>
    /// The id of the page template this region is parented to.
    /// </summary>
    public int PageTemplateId { get; set; }

    /// <summary>
    /// Region names can be any text string but will likely be 
    /// alpha-numerical human readable names like 'Heading' or 'Main Content'.
    /// Region names should be unique (non-case sensitive) irrespective of
    /// whether they are custom entity regions or not.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Indicates whether this region should apply to the Page (false) or
    /// to a CustomEntity (true). This is only relevant for Templates with 
    /// a type of PageType.CustomEntityDetails
    /// </summary>
    public bool IsCustomEntityRegion { get; set; }

    /// <summary>
    /// The page tmeplate this region is parented to
    /// </summary>
    public virtual PageTemplate PageTemplate { get; set; }

    /// <summary>
    /// The date the template was created
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// The date the template was last updated
    /// </summary>
    public DateTime UpdateDate { get; set; }

    /// <summary>
    /// In each page implementation a region can have zero or more page 
    /// blocks, these contain the dynamic content that gets rendered into
    /// the template.
    /// </summary>
    public virtual ICollection<PageVersionBlock> PageVersionBlocks { get; set; } = new List<PageVersionBlock>();
}
