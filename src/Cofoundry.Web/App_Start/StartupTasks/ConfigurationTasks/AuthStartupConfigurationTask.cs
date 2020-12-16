using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Cofoundry.Web
{
    /// <summary>
    /// Adds the asp.net auth middleware into the pipeline.
    /// </summary>
    public class AuthStartupConfigurationTask 
        : IStartupConfigurationTask
        , IRunAfterStartupConfigurationTask
    {
        public int Ordering
        {
            get { return (int)StartupTaskOrdering.Early; }
        }

        public ICollection<Type> RunAfter => new Type[] { typeof(UseRoutingStartupConfigurationTask) };

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}