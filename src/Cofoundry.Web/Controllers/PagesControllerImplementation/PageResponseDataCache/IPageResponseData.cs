﻿namespace Cofoundry.Web;

/// <summary>
/// Contains information about a page rendered dyanamically via the
/// <see cref="CofoundryPagesController"/>. This object is used to store all 
/// the information gathered during the rendering process so that it can be 
/// cached and used in other places.
/// </summary>
public interface IPageResponseData
{
    /// <summary>
    /// The main view model sent to the template for rendering
    /// </summary>
    IEditablePageViewModel Page { get; set; }

    /// <summary>
    /// The current SiteViewerMode
    /// </summary>
    VisualEditorMode VisualEditorMode { get; set; }

    /// <summary>
    /// The PageRoute for the page being displayed.
    /// </summary>
    PageRoutingInfo PageRoutingInfo { get; set; }

    /// <summary>
    /// The PageVersionRoute for the specific version of the page being displayed.
    /// </summary>
    PageVersionRoute PageVersion { get; set; }

    /// <summary>
    /// RouteInfo for the Version being viewed, be it a page or custom entity.
    /// </summary>
    IVersionRoute Version { get; set; }

    CustomEntityDefinitionSummary? CustomEntityDefinition { get; set; }

    /// <summary>
    /// True if the page has a draft version available.
    /// </summary>
    bool HasDraftVersion { get; set; }

    /// <summary>
    /// User context representing the logged in Cofoundry admin user, or
    /// null if the user is not logged into the admin auth schema. This differs
    /// from the ambient user context because the default schema may not be
    /// the Cofoundry admin auth schema.
    /// </summary>
    IUserContext? CofoundryAdminUserContext { get; set; }
}
