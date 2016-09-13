using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Cofoundry.Web.ModularMvc;
using Cofoundry.Domain;
using Cofoundry.Core;

namespace Cofoundry.Web.Admin
{
    public class UsersRouteRegistration : IRouteRegistration
    {
        private readonly IUserAreaRepository _userAreaRepository;

        public UsersRouteRegistration(
            IUserAreaRepository userAreaRepository
            )
        {
            _userAreaRepository = userAreaRepository;
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            var controllerNamespace = new string[] { typeof(UsersRouteRegistration).Namespace };

            foreach (var userArea in _userAreaRepository.GetAll())
            {
                var routePrefix = SlugFormatter.ToSlug(userArea.Name);
                var routeLibrary = new ModuleRouteLibrary(routePrefix);
                var jsRouteLibrary = new ModuleJsRouteLibrary(routeLibrary);

                routes.MapRoute(
                    "Users Cofoundry Admin Module - " + userArea.Name,
                    RouteConstants.AdminAreaPrefix + "/" + routePrefix + "-users",
                    new { controller = "UsersModule", action = "Index", userArea = userArea, Area = RouteConstants.AdminAreaName },
                    controllerNamespace
                    );
            }
        }
    }
}
