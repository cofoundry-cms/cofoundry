using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace Cofoundry.Web.Admin
{
    public class MvcAdminFilterConfigurationStartupTask : IStartupTask
    {
        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Normal; }
        }

        public void Run(IApplicationBuilder app)
        {
            //var filters = GlobalFilters.Filters;
            //filters.Add(new VisualEditorContentFilterAttribute());
        }
    }
}