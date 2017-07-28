using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Web
{
    /// <summary>
    /// Settings for managed content (e.g. pages/files/custom entities)
    /// </summary>
    public class ContentSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// A developer setting which can be used to view unpublished versions of content 
        /// without being logged into the administrator site.
        /// </summary>
        /// <remarks>
        /// Replaces a call to HttpContext.IsDebuggingEnabled in InitStateRoutingStep which used to 
        /// be used so that a developer could preview a page without putting it live, presumably when 
        /// developing content against a live db.
        /// </remarks>
        public bool AlwaysShowUnpublishedData { get; set; }
    }
}