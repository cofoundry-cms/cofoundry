using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public class RedirectResponseHelper : IRedirectResponseHelper
    {
        /// <summary>
        /// Adds querystring parameters to the redirect result.
        /// </summary>
        /// <param name="paramsToInclude">Optional list of parameters to include. If this is empty all parameters will be included</param>
        /// <returns>Modified RedirectToRouteResult object</returns>
        public RedirectToRouteResult IncludeQueryParameters(RedirectToRouteResult redirectResult, params string[] paramsToInclude)
        {
            var query = HttpContext.Current.Request.QueryString;
            foreach (string key in query.Keys)
            {
                if (!paramsToInclude.Any() || paramsToInclude.Contains(key))
                {
                    if (!redirectResult.RouteValues.ContainsKey(key))
                    {
                        redirectResult.RouteValues.Add(key, query[key]);
                    }
                }
            }

            return redirectResult;
        }

        /// <summary>
        /// Adds querystring parameters to the redirect result.
        /// </summary>
        /// <param name="paramsToInclude">Optional list of parameters to include. If this is empty all parameters will be included</param>
        /// <returns>Modified RedirectResult object</returns>
        public RedirectResult IncludeQueryParameters(RedirectResult redirectResult, params string[] paramsToInclude)
        {
            if (string.IsNullOrEmpty(redirectResult.Url)) return redirectResult;

            var urlParts = redirectResult.Url.Split('?');
            var url = urlParts.First();
            var qs = string.Join(string.Empty, urlParts.Skip(1));

            var uriQuery = HttpUtility.ParseQueryString(qs);

            var currentQuery = HttpContext.Current.Request.QueryString;
            foreach (string key in currentQuery.Keys)
            {
                if (!paramsToInclude.Any() || paramsToInclude.Contains(key))
                {
                    if (!uriQuery.AllKeys.Contains(key))
                    {
                        uriQuery.Add(key, currentQuery[key]);
                    }
                }
            }

            if (uriQuery.Count > 0)
            {
                url = url + "?" + uriQuery.ToString();
            }
            var result = new RedirectResult(url, redirectResult.Permanent);
            return result;
        }

    }
}