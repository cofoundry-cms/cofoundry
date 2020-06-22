using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Cofoundry.Web
{
    /// <summary>
    /// Adds the ASP.NET endpoint routing middleware to the pipeline.
    /// This must run after the static file handler but before 
    /// authentication is added.
    /// </summary>
    public class UseRoutingStartupConfigurationTask
        : IStartupConfigurationTask
        , IRunAfterStartupConfigurationTask
    {
        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Early; }
        }

        public ICollection<Type> RunAfter => new Type[] { typeof(StaticFileStartupConfigurationTask) };

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
        }
    }
}