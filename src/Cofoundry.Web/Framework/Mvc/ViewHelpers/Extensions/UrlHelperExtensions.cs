using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Takes a relative path and makes it absolute using the current request
        /// url as a base.
        /// </summary>
        /// <param name="relativePath">Relative path to transform</param>
        public static string ToAbsolute(this UrlHelper urlHelper, string relativePath)
        {
            var path = urlHelper.Content(relativePath);
            var url = new Uri(HttpContext.Current.Request.Url, path);

            return url.AbsoluteUri;
        }
    }
}