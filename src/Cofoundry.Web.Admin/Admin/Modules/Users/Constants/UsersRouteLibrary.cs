using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    public class UsersRouteLibrary : ModuleRouteLibrary
    {
        #region statics

        public const string RoutePrefix = "users";

        public static readonly UsersRouteLibrary Urls = new UsersRouteLibrary();

        public static readonly ModuleJsRouteLibrary Js = new ModuleJsRouteLibrary(Urls);

        #endregion

        #region constructor

        public UsersRouteLibrary()
            : base(RoutePrefix, RouteConstants.InternalModuleResourcePathPrefix)
        {
        }

        #endregion
    }
}