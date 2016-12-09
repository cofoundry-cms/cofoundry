using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorJsRouteLibrary : ModuleJsRouteLibrary
    {
        public VisualEditorJsRouteLibrary(
            ModuleRouteLibrary moduleRouteLibrary
            )
            : base(moduleRouteLibrary)
        {
        }

        #region custom routes

        public string VisualEditor
        {
            get
            {
                return Bundle("VisualEditor");
            }
        }

        #endregion
    }
}