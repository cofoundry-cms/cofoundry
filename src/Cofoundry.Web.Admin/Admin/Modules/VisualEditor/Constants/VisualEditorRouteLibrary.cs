using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorRouteLibrary : AngularModuleRouteLibrary
    {
        public const string RoutePrefix = "visual-editor";

        #region constructor

        public VisualEditorRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion

        #region resources

        public string VisualEditorToolbarViewPath()
        {
            return ResourcePrefix + "mvc/views/Toolbar.cshtml";
        }


        #endregion
    }
}