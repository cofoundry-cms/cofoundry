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
        /// The main view model sent to the template for rendering
        /// </summary>
        public IEditablePageViewModel Page { get; set; }

        /// <summary>
        /// The current VisualEditorMode
        /// </summary>
        public VisualEditorMode VisualEditorMode { get; set; }

        public PageRoutingInfo PageRoutingInfo { get; set; }

        public PageVersionRoute PageVersion { get; set; }

        /// <summary>
        /// RouteInfo for the Version being viewed, be it a page or custom entity.
        /// </summary>
        public IVersionRoute Version { get; set; }

        public CustomEntityDefinitionSummary CustomEntityDefinition { get; set; }

        public bool HasDraftVersion { get; set; }

        public bool IsCustomEntityRoute { get; set; }

        /// <summary>
        /// Has permission to update the page or custom entity requested
        /// to be edited.
        /// </summary>
        public bool HasEntityUpdatePermission { get; set; }

        /// <summary>
        /// Has permission to publish the page or custom entity requested
        /// to be edited.
        /// </summary>
        public bool HasEntityPublishPermission { get; set; }
    }
}