using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <remarks>
    /// The db supports a web directory having different paths per
    /// locale, but this isn't used at the admin UI level and will
    /// likely be revised at a future date.
    /// </remarks>
    public class WebDirectoryRouteLocale
    {
        public int LocaleId { get; set; }

        public string UrlPath { get; set; }

        public string FullUrlPath { get; set; }
    }
}
