using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private const string VIEW_FORMAT = "~" + RouteConstants.InternalModuleResourcePathPrefix + "{0}/MVC/Views/{1}.cshtml";

        public static string View(string controllerName, string viewName)
        {
            if (controllerName == null) throw new ArgumentNullException(nameof(controllerName));
            if (string.IsNullOrWhiteSpace(controllerName)) throw new ArgumentEmptyException(nameof(controllerName));
            if (viewName == null) throw new ArgumentNullException(nameof(viewName));
            if (string.IsNullOrWhiteSpace(viewName)) throw new ArgumentEmptyException(nameof(viewName));

            return string.Format(VIEW_FORMAT, controllerName, viewName);
        }
    }
}