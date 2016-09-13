using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class UsersModuleController : BaseAdminMvcController
    {
        private static readonly Dictionary<string, string> EmptyTerms = new Dictionary<string, string>();

        public ActionResult Index(IUserArea userArea)
        {
            var options = new UsersModuleOptions()
            {
                UserAreaCode = userArea.UserAreaCode,
                Name = userArea.Name,
                AllowPasswordLogin = userArea.AllowPasswordLogin,
                UseEmailAsUsername = userArea.UseEmailAsUsername
            };

            return View("~/Admin/Modules/Users/Mvc/Views/Index.cshtml", options);
        }
    }
}