using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;

namespace Cofoundry.BasicTestSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.local.json", optional: true);
            });
    }
}
