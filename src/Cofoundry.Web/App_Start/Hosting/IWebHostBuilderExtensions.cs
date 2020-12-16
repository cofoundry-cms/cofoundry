using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    public static class IWebHostBuilderExtensions
    {
        /// <summary>
        /// Uses an optional appsettings.local.json configuration file
        /// for local developer configuration, only when in the developer 
        /// environment. This local configuration file should not be checked 
        /// into version control.
        /// </summary>
        public static IWebHostBuilder UseLocalConfigFile(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureAppConfiguration((hostContext, config) =>
            {
                if (hostContext.HostingEnvironment.IsDevelopment())
                {
                    config.AddJsonFile("appsettings.local.json", optional: true);
                }
            });

            return webHostBuilder;
        }
    }
}
