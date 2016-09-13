using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Keeps track of any variables that are required through the
    /// Page routing pipeline.
    /// </summary>
    public class PageActionRoutingState
    {
        /// <summary>
        /// Locale of the page, or null if locales aren't supported.
        /// </summary>
        public ActiveLocale Locale { get; set; }

        /// <summary>
        /// Represents the input parameters of the Page action.
        /// </summary>
        public PageActionInputParameters InputParameters { get; set; }

        /// <summary>
        /// The current user context for the request.
        /// </summary>
        public IUserContext UserContext { get; set; }

        /// <summary>
        /// The Requested site viewer mode.
        /// </summary>
        public SiteViewerMode SiteViewerMode { get; set; }

        /// <summary>
        /// A PageRoute if one is found during the pipeline process.
        /// </summary>
        public PageRoutingInfo PageRoutingInfo { get; set; }

        /// <summary>
        /// The resulting if one is found during the pipeline process.
        /// </summary>
        public PageRenderDetails PageData { get; set; }

        /// <summary>
        /// An action result to return from the Page action. Set this at any point in the
        /// pipline and it will be returned after the current method has finished executing.
        /// </summary>
        public ActionResult Result { get; set; }
    }
}
