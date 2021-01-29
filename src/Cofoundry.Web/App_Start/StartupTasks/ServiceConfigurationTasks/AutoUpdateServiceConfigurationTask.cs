using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// A simple task to add the AutoUpdate hosted service, which manages the 
    /// auto-update process when the application starts up.
    /// </summary>
    public class AutoUpdateServiceConfigurationTask : IStartupServiceConfigurationTask
    {
        public void ConfigureServices(IMvcBuilder mvcBuilder)
        {
            mvcBuilder.Services.AddHostedService<AutoUpdateHostedService>();
        }
    }
}