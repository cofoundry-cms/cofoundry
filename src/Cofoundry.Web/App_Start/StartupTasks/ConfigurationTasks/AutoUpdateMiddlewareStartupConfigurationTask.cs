using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// A simple task to add the AutoUpdateMiddleware to the pipeline
    /// which runs the auto-update process when the application starts up.
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