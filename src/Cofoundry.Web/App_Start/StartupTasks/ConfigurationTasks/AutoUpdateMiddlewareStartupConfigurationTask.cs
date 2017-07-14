using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// A simple task to add the AutoUpdateMiddleware to the owin pipeline
    /// so it runs first.
    /// </summary>
    public class AutoUpdateMiddlewareStartupConfigurationTask : IStartupConfigurationTask
    {
        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Early; }
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<AutoUpdateMiddleware>();
        }
    }
}