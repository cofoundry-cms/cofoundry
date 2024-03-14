namespace Cofoundry.Web;

/// <summary>
/// Contains information about a page rendered dyanamically via the
/// PagesController. This object is used to store all the information 
/// gathered during the rendering process so that it can be cached and used 
/// in other places. Specifically this is done for rendering the admin site
/// viewer from SiteViewerContentFilterAttribute but this could potentially 
/// be used in other places like other filters or view helpers.
/// </summary>
public class PageResponseData : IPageResponseData
{
    /// <inheritdoc/>
    public required IEditablePageViewModel Page { get; set; }

    /// <inheritdoc/>
    public required VisualEditorMode VisualEditorMode { get; set; }

    /// <inheritdoc/>
    public required PageRoutingInfo PageRoutingInfo { get; set; }

    /// <inheritdoc/>
    public required PageVersionRoute PageVersion { get; set; }

    /// <inheritdoc/>
    public required IVersionRoute Version { get; set; }

    /// <inheritdoc/>
    public CustomEntityDefinitionSummary? CustomEntityDefinition { get; set; }

    /// <inheritdoc/>
    public bool HasDraftVersion { get; set; }

    /// <inheritdoc/>
    public IUserContext? CofoundryAdminUserContext { get; set; }
}
