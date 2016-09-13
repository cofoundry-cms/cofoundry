using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class PageRoutingHelper
    {
        public PageRoutingHelper(
            IEnumerable<PageRoute> pageRoutes,
            IEnumerable<WebDirectoryRoute> webDirectoryRoutes,
            PageRoute currentPageRoute,
            SiteViewerMode siteViewerMode,
            int? currentPageVersionId
            )
        {
            PageRoutes = pageRoutes;
            WebDirectoryRoutes = webDirectoryRoutes;
            CurrentPageRoute = currentPageRoute;
            SiteViewerMode = siteViewerMode;
            CurrentPageVersionId = currentPageVersionId;
        }

        public SiteViewerMode SiteViewerMode { get; private set; }
        public PageRoute CurrentPageRoute { get; private set; }
        public IEnumerable<PageRoute> PageRoutes { get; private set; }
        public IEnumerable<WebDirectoryRoute> WebDirectoryRoutes { get; private set; }
        public int? CurrentPageVersionId { get; private set; }

    }
}