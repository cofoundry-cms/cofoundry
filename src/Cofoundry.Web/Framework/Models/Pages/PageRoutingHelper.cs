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
            VisualEditorMode visualEditorMode,
            int? currentPageVersionId
            )
        {
            PageRoutes = pageRoutes;
            WebDirectoryRoutes = webDirectoryRoutes;
            CurrentPageRoute = currentPageRoute;
            VisualEditorMode = visualEditorMode;
            CurrentPageVersionId = currentPageVersionId;
        }

        public VisualEditorMode VisualEditorMode { get; private set; }
        public PageRoute CurrentPageRoute { get; private set; }
        public IEnumerable<PageRoute> PageRoutes { get; private set; }
        public IEnumerable<WebDirectoryRoute> WebDirectoryRoutes { get; private set; }
        public int? CurrentPageVersionId { get; private set; }

    }
}