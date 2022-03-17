namespace Cofoundry.Web;

/// <summary>
/// Optional interface you can use to decorate an instance of
/// IPageBlockTypeDisplayData to add some contextual information about the
/// page the block is parented to.
/// </summary>
public interface IPageBlockWithParentPageData : IPageBlockTypeDisplayModel
{
    /// <summary>
    /// Provides access to the page containing the block. 
    /// </summary>
    IEditablePageViewModel ParentPage { get; set; }
}
