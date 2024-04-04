using System.Diagnostics.CodeAnalysis;

namespace Cofoundry.Web;

public class PageViewModel : IPageViewModel
{
    private PageRenderDetails? _page { get; set; }
    public PageRenderDetails Page
    {
        get
        {
            PagePropertyNullCheck();
            return _page;
        }
        set
        {
            _page = value;
            _pageTitle = _page.Title;
            _metaDescription = _page.MetaDescription;
        }
    }

    public bool IsPageEditMode { get; set; }

    public string _pageTitle = string.Empty;
    public string PageTitle
    {
        get
        {
            PagePropertyNullCheck();
            return _pageTitle;
        }
        set
        {
            PagePropertyNullCheck();
            _pageTitle = value;
        }
    }

    public string? _metaDescription;
    public string? MetaDescription
    {
        get
        {
            PagePropertyNullCheck();
            return _metaDescription;
        }
        set
        {
            PagePropertyNullCheck();
            _metaDescription = value;
        }
    }

    private PageRoutingHelper? _pageRoutingHelper;
    public PageRoutingHelper PageRoutingHelper
    {
        get => _pageRoutingHelper ?? throw ViewModelPropertyNotInitializedException.Create<PageViewModel>(nameof(PageRoutingHelper));
        set => _pageRoutingHelper = value;
    }

    [MemberNotNull(nameof(_page))]
    private void PagePropertyNullCheck()
    {
        if (_page == null)
        {
            throw ViewModelPropertyNotInitializedException.Create<PageViewModel>(nameof(Page));
        }
    }
}
