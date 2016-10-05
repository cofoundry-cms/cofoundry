using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class SharedJsRouteLibrary : ModuleJsRouteLibrary
    {
        public SharedJsRouteLibrary(
            ModuleRouteLibrary moduleRouteLibrary
            )
            : base(moduleRouteLibrary)
        {
        }

        #region custom routes

        public string Html5Shiv
        {
            get
            {
                return Bundle("html5shiv");
            }
        }

        #endregion
    }
}