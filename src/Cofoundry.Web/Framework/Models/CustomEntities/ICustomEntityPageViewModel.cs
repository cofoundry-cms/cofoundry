namespace Cofoundry.Web;

/// <summary>
/// A page view model class for a custom entity page that includes detailed custom
/// entity data. The default implementation, can be overridden by implementing a 
/// custom <see cref="IPageViewModelFactory"/>.
/// </summary>
/// <typeparam name="TDisplayModel">
/// The type of view model used to represent the custom entity data model when formatted for display.
/// </typeparam>
public interface ICustomEntityPageViewModel<TDisplayModel>
    : IPageWithMetaDataViewModel, IEditablePageViewModel, IPageRoutableViewModel
    where TDisplayModel : ICustomEntityPageDisplayModel
{
    CustomEntityRenderDetailsViewModel<TDisplayModel> CustomEntity { get; set; }
}
