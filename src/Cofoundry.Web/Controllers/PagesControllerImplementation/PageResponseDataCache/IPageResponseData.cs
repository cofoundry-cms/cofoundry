using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    public interface IPageResponseData
    {
        /// <summary>
        /// The main view model sent to the template for rendering
        /// </summary>
        IEditablePageViewModel Page { get; set; }

        /// <summary>
        /// The current SiteViewerMode
        /// </summary>
        VisualEditorMode SiteViewerMode { get; set; }

        PageRoutingInfo PageRoutingInfo { get; set; }

        PageVersionRoute PageVersion { get; set; }

        /// <summary>
        /// RouteInfo for the Version being viewed, be it a page or custom entity.
        /// </summary>
        IVersionRoute Version { get; set; }

        CustomEntityDefinitionSummary CustomEntityDefinition { get; set; }

        bool HasDraftVersion { get; set; }

        bool IsCustomEntityRoute { get; set; }
    }
}