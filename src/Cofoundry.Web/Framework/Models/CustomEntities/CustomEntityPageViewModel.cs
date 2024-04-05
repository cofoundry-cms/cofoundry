using System.Diagnostics.CodeAnalysis;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="ICustomEntityPageViewModel{TDisplayModel}"/>. This
/// implementation can be overridden by implementing a custom <see cref="IPageViewModelFactory"/>.
/// </summary>
public class CustomEntityPageViewModel<TDisplayModel> : ICustomEntityPageViewModel<TDisplayModel>
    where TDisplayModel : ICustomEntityPageDisplayModel
{
    public string _pageTitle = string.Empty;
    public string PageTitle
    {
        get
        {
            CustomEntityModelPropertyNullCheck();
            return _pageTitle;
        }
        set
        {
            CustomEntityModelPropertyNullCheck();
            _pageTitle = value;
        }
    }

    public string? _metaDescription;
    public string? MetaDescription
    {
        get
        {
            CustomEntityModelPropertyNullCheck();
            return _metaDescription;
        }
        set
        {
            CustomEntityModelPropertyNullCheck();
            _metaDescription = value;
        }
    }

    /// <summary>
    /// Data about the page this custom entity instance is hosted in. For custom
    /// entity pages the page entity forms a template for each custom entity
    /// </summary>
    private PageRenderDetails? _page { get; set; }
    public PageRenderDetails Page
    {
        get => _page ?? throw ViewModelPropertyNotInitializedException.Create<CustomEntityPageViewModel<TDisplayModel>>(nameof(Page));
        set => _page = value;
    }

    private CustomEntityRenderDetailsViewModel<TDisplayModel>? _customEntity { get; set; }
    public CustomEntityRenderDetailsViewModel<TDisplayModel> CustomEntity
    {
        get
        {
            CustomEntityModelPropertyNullCheck();
            return _customEntity;
        }
        set
        {
            _customEntity = value;
            _pageTitle = _customEntity.Model.PageTitle;
            _metaDescription = _customEntity.Model.MetaDescription;
        }
    }

    public bool IsPageEditMode { get; set; }

    private PageRoutingHelper? _pageRoutingHelper;
    public PageRoutingHelper PageRoutingHelper
    {
        get => _pageRoutingHelper ?? throw ViewModelPropertyNotInitializedException.Create<CustomEntityPageViewModel<TDisplayModel>>(nameof(PageRoutingHelper));
        set => _pageRoutingHelper = value;
    }

    [MemberNotNull(nameof(_customEntity))]
    private void CustomEntityModelPropertyNullCheck()
    {
        if (_customEntity == null)
        {
            throw ViewModelPropertyNotInitializedException.Create<CustomEntityPageViewModel<TDisplayModel>>(nameof(CustomEntity));
        }

        EntityInvalidOperationException.ThrowIfNull(_customEntity, _customEntity.Model);
    }
}
