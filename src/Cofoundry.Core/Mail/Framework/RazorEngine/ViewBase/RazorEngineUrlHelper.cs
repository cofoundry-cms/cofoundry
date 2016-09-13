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
        private readonly ISiteUriResolver _siteUriResolver;

        public RazorEngineUrlHelper(
            ISiteUriResolver siteUriResolver
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