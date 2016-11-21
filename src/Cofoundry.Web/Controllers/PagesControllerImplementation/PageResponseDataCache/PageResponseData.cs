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
    public class PageResponseData : IPageResponseData
    {
        /// <summary>
        /// The main view model sent to the template for rendering
        /// </summary>
        public IEditablePageViewModel Page { get; set; }

        /// <summary>
        /// The current SiteViewerMode
        /// </summary>
        public SiteViewerMode SiteViewerMode { get; set; }

        public PageRoutingInfo PageRoutingInfo { get; set; }

        public PageVersionRoute PageVersion { get; set; }

        /// <summary>
        /// RouteInfo for the Version being viewed, be it a page or custom entity.
        /// </summary>
        public IVersionRoute Version { get; set; }

        public CustomEntityDefinitionSummary CustomEntityDefinition { get; set; }

        public bool HasDraftVersion { get; set; }

        //public bool IsEditTypeSwitchRequired()
        //{
        //    return PageRoutingInfo.CustomEntityRoute != null
        //        && PageVersion.HasCustomEntityModuleSections
        //        && PageVersion.HasPageModuleSections;
        //}

        //public bool CanEditPage()
        //{
        //    return PageVersion.HasCustomEntityModuleSections
        //        || PageVersion.HasPageModuleSections;
        //}
    }
}