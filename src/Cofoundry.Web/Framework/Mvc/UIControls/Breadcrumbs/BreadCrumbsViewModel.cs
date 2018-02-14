using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// A view model for displaying breadcrumbs on a page.
    /// </summary>
    public class BreadcrumbsViewModel
    {
        public ICollection<BreadcrumbViewModel> Breadcrumbs { get; set; }
    }
}