namespace Cofoundry.Domain;

/// <summary>
/// Optional interface you can use to decorate an instance of
/// IPageBlockTypeDisplayModel to add some contextual information to your block display
/// view model.
/// </summary>
public interface IListablePageBlockTypeDisplayModel : IPageBlockTypeDisplayModel
{
    /// <summary>
    /// Gives information about the collection that a block is in.
    /// </summary>
    ListablePageBlockRenderContext ListContext { get; set; }
}
