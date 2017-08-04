using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class VisualEditorPageViewModel
    {
        public bool IsStaticPage { get; set; }

        public VisualEditorMode VisualEditorMode { get; set; }

        public PageRoutingInfo PageRoutingInfo { get; set; }

        public PageVersionRoute PageVersion { get; set; }

        /// <summary>
        /// RouteInfo for the Version being viewed, be it a page or custom entity.
        /// </summary>
        public IVersionRoute Version { get; set; }

        public CustomEntityDefinitionSummary CustomEntityDefinition { get; set; }

        public bool HasDraftVersion { get; set; }

        public bool IsEditTypeSwitchRequired()
        {
            return PageRoutingInfo != null
                && PageVersion != null
                && PageRoutingInfo.CustomEntityRoute != null
                && PageVersion.HasCustomEntityRegions
                && PageVersion.HasPageRegions;
        }

        public bool CanEditPage()
        {
            return PageVersion != null
                && (PageVersion.HasCustomEntityRegions
                || PageVersion.HasPageRegions);
        }
    }
}