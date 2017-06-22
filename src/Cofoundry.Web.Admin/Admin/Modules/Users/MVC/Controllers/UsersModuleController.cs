using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class UsersModuleController : BaseAdminMvcController
    {
        private static readonly Dictionary<string, string> EmptyTerms = new Dictionary<string, string>();

        public ActionResult Index()
        {
            var userArea = RouteData.DataTokens["UserArea"] as IUserAreaDefinition;
            var options = new UsersModuleOptions()
            {
                UserAreaCode = userArea.UserAreaCode,
                Name = userArea.Name,
                AllowPasswordLogin = userArea.AllowPasswordLogin,
                UseEmailAsUsername = userArea.UseEmailAsUsername
            };

            var viewPath = ViewPathFormatter.View("Users", nameof(Index));
            return View(viewPath, options);
        }
    }
}