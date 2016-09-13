using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class AuthJSRouteLibrary : ModuleJsRouteLibrary
    {
        #region constructor

        public AuthJSRouteLibrary(
            ModuleRouteLibrary moduleRouteLibrary
            )
            : base(moduleRouteLibrary)
        {
        }

        #endregion

        #region bundles

        public string Login
        {
            get
            {
                return Bundle("login");
            }
        }

        public string ForgotPassword
        {
            get
            {
                return Bundle("forgotpassword");
            }
        }

        public string ChangePassword
        {
            get
            {
                return Bundle("changepassword");
            }
        }

        #endregion
    }
}