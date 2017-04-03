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
    public class AutoUpdateMiddlewareRegistrationStartupTask : IStartupTask
    {
        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Early; }
        }

        public void Run(IApplicationBuilder app)
        {
            app.UseMiddleware<AutoUpdateMiddleware>();
        }
    }
}