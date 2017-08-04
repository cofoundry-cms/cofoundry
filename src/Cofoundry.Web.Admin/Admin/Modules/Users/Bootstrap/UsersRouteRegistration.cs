using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Core;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;

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

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            foreach (var userArea in _userAreaRepository.GetAll())
            {
                var routePrefix = SlugFormatter.ToSlug(userArea.Name);

                routeBuilder.MapRoute(
                    "Users Cofoundry Admin Module - " + userArea.Name,
                    RouteConstants.AdminAreaPrefix + "/" + routePrefix + "-users",
                    new { controller = "UsersModule", action = "Index", Area = RouteConstants.AdminAreaName },
                    null,
                    new { UserArea = userArea }
                    );
            }
        }
    }
}
