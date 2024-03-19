namespace Cofoundry.Web;

public class UninitializedEditablePageViewModel : IEditablePageViewModel
{
    public PageRenderDetails Page
    {
        get
        {
            return PageRenderDetails.Uninitialized;
        }
        set
        {
            throw new NotImplementedException($"{nameof(UninitializedEditablePageViewModel)} is read-only");
        }
    }

    public bool IsPageEditMode
    {
        get
        {
            return false;
        }
        set
        {
            throw new NotImplementedException($"{nameof(UninitializedEditablePageViewModel)} is read-only");
        }
    }

    /// <summary>
    /// A placeholder value to use for not-nullable values that you
    /// know will be initialized in later code. This value should not
    /// be used in data post-initialization.
    /// </summary>
    public static readonly UninitializedEditablePageViewModel Instance = new();
}
