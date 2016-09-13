using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class AuthCssRouteLibrary
    {
        #region constructor

        private readonly ModuleStaticContentRouteLibrary _moduleStaticContentRouteLibrary;

        public AuthCssRouteLibrary(
            ModuleStaticContentRouteLibrary moduleStaticContentRouteLibrary
            )
        {
            _moduleStaticContentRouteLibrary = moduleStaticContentRouteLibrary;
        }

        #endregion

        public string Main
        {
            get
            {
                return Bundle("main");
            }
        }

        public string Bundle(string path)
        {
            return "~" + _moduleStaticContentRouteLibrary.Url("css/" + path);
        }
    }
}