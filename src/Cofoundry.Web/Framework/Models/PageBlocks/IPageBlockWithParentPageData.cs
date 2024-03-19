namespace Cofoundry.Web;

/// <summary>
/// Optional interface you can use to decorate an instance of
/// <see cref="IPageBlockTypeDisplayModel"/> to add some contextual 
/// information about the page the block is parented to.
/// </summary>
public interface IPageBlockWithParentPageData : IPageBlockTypeDisplayModel
{
    /// <summary>
    /// Provides access to the page containing the block. You can safely
    /// initialize this value with <see cref="UninitializedEditablePageViewModel.Instance"/>
    /// to work around nullable reference type warnings.
    /// </summary>
    IEditablePageViewModel ParentPage { get; set; }
}
