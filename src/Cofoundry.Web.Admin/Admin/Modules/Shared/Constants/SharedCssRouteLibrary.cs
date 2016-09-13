using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class SharedCssRouteLibrary
    {
        #region constructor

        private readonly ModuleStaticContentRouteLibrary _moduleStaticContentRouteLibrary;

        public SharedCssRouteLibrary(
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

        public string Html5Shiv
        {
            get
            {
                return Bundle("html5shiv");
            }
        }

        public string Bundle(string path)
        {
            return "~" + _moduleStaticContentRouteLibrary.Url("css/" + path);
        }

    }
}