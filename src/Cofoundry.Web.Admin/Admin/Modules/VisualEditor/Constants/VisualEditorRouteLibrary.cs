using System;
using System.Collections.Generic;
using System.Linq;

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
            return ResourcePrefix + "MVC/Views/Toolbar.cshtml";
        }


        #endregion
    }
}