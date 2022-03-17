namespace Cofoundry.BasicTestSite;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(ConfigureWebHost);

    public static void ConfigureWebHost(IWebHostBuilder webBuilder)
    {
        webBuilder
            .UseStartup<Startup>()
            .UseLocalConfigFile();
    }
}
