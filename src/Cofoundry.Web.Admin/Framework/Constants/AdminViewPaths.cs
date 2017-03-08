using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// This is a little helper to make it easier to reference views by
    /// full path without having to go through the view locator. This means
    /// we don't have to register the admin view path with the view engine 
    /// so we can avoid potential conflicts.
    /// </summary>
    internal static class ViewPathFormatter
    {
        private const string VIEW_FORMAT = "~" + RouteConstants.InternalModuleResourcePathPrefix + "{0}/Mvc/Views/{1}.cshtml";

        public static string View(string controllerName, string viewName)
        {
            Condition.Requires(controllerName).IsNotNullOrWhiteSpace();
            Condition.Requires(viewName).IsNotNullOrWhiteSpace();

            return string.Format(VIEW_FORMAT, controllerName, viewName);
        }
    }
}