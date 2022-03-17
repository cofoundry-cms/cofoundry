namespace Cofoundry.Web;

public interface IEditablePageViewModel
{
    PageRenderDetails Page { get; set; }

    bool IsPageEditMode { get; set; }
}
