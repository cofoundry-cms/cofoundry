namespace Cofoundry.BasicTestSite;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddControllersWithViews()
            .AddCofoundry(Configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Https redirection / cookie policy and hsts is all non-cofoundry stuff
        if (!env.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCookiePolicy();

        app.UseCofoundry();

    }
}
