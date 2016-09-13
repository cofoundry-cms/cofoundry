using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Adds a few MVC filters required by Cofoundry to the global configuration
    /// </summary>
    public class MvcFilterConfigurationStartupTask : IStartupTask
    {
        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Normal; }
        }

        public void Run(IAppBuilder app)
        {
            var filters = GlobalFilters.Filters;
            filters.Add(new WhitespaceFilterAttribute());
            filters.Add(new ExceptionLogAttribute());
        }
    }
}