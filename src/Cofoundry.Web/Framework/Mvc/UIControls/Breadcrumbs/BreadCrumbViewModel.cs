using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// View model for an individual breadcrumb URL
    /// </summary>
    public class BreadcrumbViewModel
    {
        public string Title { get; set; }

        public string Href { get; set; }

        public bool HasHref
        {
            get
            {
                return !string.IsNullOrEmpty(Href);
            }
        }
    }
}