namespace Cofoundry.Web;

public interface ICustomEntityPageViewModel<TDisplayModel>
    : IPageWithMetaDataViewModel, IEditablePageViewModel, IPageRoutableViewModel
    where TDisplayModel : ICustomEntityPageDisplayModel
{
    CustomEntityRenderDetailsViewModel<TDisplayModel> CustomEntity { get; set; }
}
