using Cofoundry.Core.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Adds some url resolver helpers for mail templating.
    /// </summary>
    public class RazorEngineUrlHelper
    {
        private readonly ISiteUrlResolver _siteUriResolver;

        public RazorEngineUrlHelper(
            ISiteUrlResolver siteUriResolver
            )
        {
            _siteUriResolver = siteUriResolver;
        }

        /// <summary>
        /// Converts a relative Url to a fully qualified absolute url.
        /// </summary>
        public string ToAbsolute(string url)
        {
            return _siteUriResolver.MakeAbsolute(url);
        }
    }
}