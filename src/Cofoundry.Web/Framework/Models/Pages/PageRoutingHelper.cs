namespace Cofoundry.Web;

public class PageRoutingHelper
{
    public PageRoutingHelper(
        IReadOnlyCollection<PageRoute> pageRoutes,
        IReadOnlyCollection<PageDirectoryRoute> pageDirectoryRoutes,
        PageRoute currentPageRoute,
        VisualEditorMode visualEditorMode,
        int? currentPageVersionId
        )
    {
        PageRoutes = pageRoutes;
        PageDirectoryRoutes = pageDirectoryRoutes;
        CurrentPageRoute = currentPageRoute;
        VisualEditorMode = visualEditorMode;
        CurrentPageVersionId = currentPageVersionId;
    }

    public VisualEditorMode VisualEditorMode { get; private set; }

    public PageRoute CurrentPageRoute { get; private set; }

    public IReadOnlyCollection<PageRoute> PageRoutes { get; private set; }

    public IReadOnlyCollection<PageDirectoryRoute> PageDirectoryRoutes { get; private set; }

    public int? CurrentPageVersionId { get; private set; }
}
