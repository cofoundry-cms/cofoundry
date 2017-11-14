using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
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
        /// <summary>
        /// The main view model sent to the template for rendering.
        /// </summary>
        public IEditablePageViewModel Page { get; set; }

        /// <summary>
        /// The current VisualEditorMode.
        /// </summary>
        public VisualEditorMode VisualEditorMode { get; set; }

        /// <summary>
        /// The PageRoute for the page being displayed.
        /// </summary>
        public PageRoutingInfo PageRoutingInfo { get; set; }

        /// <summary>
        /// The PageVersionRoute for the specific version of the page being displayed.
        /// </summary>
        public PageVersionRoute PageVersion { get; set; }

        /// <summary>
        /// RouteInfo for the Version being viewed, be it a page or custom entity.
        /// </summary>
        public IVersionRoute Version { get; set; }

        public CustomEntityDefinitionSummary CustomEntityDefinition { get; set; }

        /// <summary>
        /// True if the page has a draft version available.
        /// </summary>
        public bool HasDraftVersion { get; set; }

        /// <summary>
        /// User context representing the logged in Cofoundry admin user, or
        /// null if the user is not logged into the admin auth schema. This differs
        /// from the ambient user context because the default schema may not be
        /// the Cofoundry admin auth schema.
        /// </summary>
        public IUserContext CofoundryAdminUserContext { get; set; }
    }
}