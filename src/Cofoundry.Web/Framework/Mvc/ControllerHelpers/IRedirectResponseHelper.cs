using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public interface IRedirectResponseHelper
    {
        /// <summary>
        /// Adds querystring parameters to the redirect result.
        /// </summary>
        /// <param name="paramsToInclude">Optional list of parameters to include. If this is empty all parameters will be included</param>
        /// <returns>Modified RedirectToRouteResult object</returns>
        RedirectToRouteResult IncludeQueryParameters(RedirectToRouteResult redirectResult, params string[] paramsToInclude);

        /// <summary>
        /// Adds querystring parameters to the redirect result.
        /// </summary>
        /// <param name="paramsToInclude">Optional list of parameters to include. If this is empty all parameters will be included</param>
        /// <returns>Modified RedirectResult object</returns>
        RedirectResult IncludeQueryParameters(RedirectResult redirectResult, params string[] paramsToInclude);
    }
}