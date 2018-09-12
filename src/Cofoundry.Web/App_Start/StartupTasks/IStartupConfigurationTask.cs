using Cofoundry.Core;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Represents a task that runs in the Cofoundry startup and 
    /// initialization pipeline.
    /// </summary>
    public interface IStartupConfigurationTask : IOrderedTask
    {
        /// <summary>
        /// Executes the configuration task
        /// </summary>
        void Configure(IApplicationBuilder app);
    }
}