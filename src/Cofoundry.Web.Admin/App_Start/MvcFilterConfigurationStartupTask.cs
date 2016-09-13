using Owin;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Cofoundry.Web.Admin
{
    public class MvcAdminFilterConfigurationStartupTask : IStartupTask
    {
        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Normal; }
        }

        public void Run(IAppBuilder app)
        {
            var filters = GlobalFilters.Filters;
            filters.Add(new SiteViewerContentFilterAttribute());
        }
    }
}