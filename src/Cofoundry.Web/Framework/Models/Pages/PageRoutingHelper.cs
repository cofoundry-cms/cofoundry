using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class PageRoutingHelper
    {
        public PageRoutingHelper(
            IEnumerable<PageRoute> pageRoutes,
            IEnumerable<PageDirectoryRoute> pageDirectoryRoutes,
            PageRoute currentPageRoute,
            VisualEditorMode visualEditorMode,
            int? currentPageVersionId
            )
        {
            PageRoutes = pageRoutes;
            PageDirectoryRoutes = pageDirectoryRoutes;
            CurrentPageRoute = currentPageRoute;
            VisualEditorMode = visualEditorMode;
            CurrentPageVersionId = currentPageVersionId;
        }

        public VisualEditorMode VisualEditorMode { get; private set; }

        public PageRoute CurrentPageRoute { get; private set; }

        public IEnumerable<PageRoute> PageRoutes { get; private set; }

        public IEnumerable<PageDirectoryRoute> PageDirectoryRoutes { get; private set; }

        public int? CurrentPageVersionId { get; private set; }
    }
}