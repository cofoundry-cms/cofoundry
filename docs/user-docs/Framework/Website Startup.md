When using Cofoundry in an asp.net web application, Cofoundry needs to manage the application startup process so that it can make sure everything is registered in the correct order and allow plugins to self-register.

Integrating Cofoundry into your site is easy, simply add the `.AddCofoundry(IConfiguration)` and `.UseCofoundry()` extension methods to your Program.cs file.

**Example Program.cs**

```csharp
using Cofoundry.Web;

var builder = WebApplication.CreateBuilder(args);

// Register Cofoundry with the DI container. 
// Must be run after AddMvc(), AddControllersWithViews() or similar method that returns IMvcBuilder
builder.Services
    .AddMvc()
    .AddCofoundry(builder.Configuration);

var app = builder.Build();

// You can register other middleware as normal
app.UseHttpsRedirection();

// Register Cofoundry into the pipeline. As part of this process it also initializes 
// the MVC middleware and runs additional startup tasks.
app.UseCofoundry();

app.Run();
```

## What happens at startup?

### AddCofoundry(IConfiguration)

This method sets up the dependency resolver for Cofoundry, registering all internal components, modules and any auto-registered types such as command and query handlers. `AddCofoundry(IConfiguration)` is called after `AddControllersWithViews()` which allows you to customize your MVC configuration independently.

Once dependencies are registered, Cofoundry will look for classes that implement `IStartupServiceConfigurationTask` and execute them. Cofoundry itself only includes a couple of configuration tasks, but plugin developers can use this as an integration point.

- **[AutoUpdateServiceConfigurationTask:](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Web/App_Start/StartupTasks/ServiceConfigurationTasks/AutoUpdateServiceConfigurationTask.cs)** Sets up the auto-update hosted service.
- **[CofoundryStartupServiceConfigurationTask:](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Web/App_Start/StartupTasks/ServiceConfigurationTasks/CofoundryStartupServiceConfigurationTask.cs)** Sets up auth (user areas) and configures a number of MVC settings

See the [Startup Tasks](Startup-Tasks) documentation for information on creating your own startup tasks.

### UseCofoundry()

This method registers Cofoundry into the application pipeline by running all registered `IStartupConfigurationTask` implementations in order. 

Here is a list of tasks bundled in Cofoundry (note that plugins authors may inject their own startup tasks):

- **[ErrorHandlingMiddlewareConfigurationTask:](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Web/App_Start/StartupTasks/ConfigurationTasks/ErrorHandlingMiddlewareConfigurationTask.cs)** Configures the error pages returned by the application for both exceptions and error status codes such as 404s. This is done using the built-in the ASP.NET error handling and status code pages middleware. For more information see the docs for [Custom Error Pages](/content-management/custom-error-pages)
- **[AuthStartupConfigurationTask:](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Web/App_Start/StartupTasks/ConfigurationTasks/AuthStartupConfigurationTask.cs)** Adds the asp.net auth middleware into the pipeline.
- **[AutoUpdateMiddlewareStartupConfigurationTask:](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Web/App_Start/StartupTasks/ConfigurationTasks/AutoUpdateMiddlewareStartupConfigurationTask.cs)** Initializes the [Auto Update](Auto-Update) process.
- **[JsonConverterStartupConfigurationTask:](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Web/App_Start/StartupTasks/ConfigurationTasks/JsonConverterStartupConfigurationTask.cs)** Configures the default JsonSerialization settings using `IJsonSerializerSettingsFactory`
- **[StaticFileStartupConfigurationTask:](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Web/App_Start/StartupTasks/ConfigurationTasks/StaticFiles/StaticFileStartupConfigurationTask.cs)** Adds the asp.net static files middleware with a default configuration. You can customize this by overriding the [default](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Web/App_Start/StartupTasks/ConfigurationTasks/StaticFiles/DefaultStaticFileOptionsConfiguration.cs) `IStaticFileOptionsConfiguration` implementation using [DI](dependency-injection#overriding-registrations).
- **[MessageAggregatorStartupConfigurationTask:](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Web/App_Start/StartupTasks/ConfigurationTasks/MessageAggregatorStartupConfigurationTask.cs)** Bootstraps the [Message Aggregator](Message-Aggregator), registering `IMessageSubscriptionRegistration` classes
- **[UseRoutingStartupConfigurationTask:](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Web/App_Start/StartupTasks/ConfigurationTasks/UseRoutingStartupConfigurationTask.cs)** Adds the ASP.NET endpoint routing middleware to the pipeline
- **[AddEndpointRoutesConfigurationTask:](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Web/App_Start/StartupTasks/ConfigurationTasks/AddEndpointRoutesStartupConfigurationTask.cs)** Configures [Cofoundry routing](/content-management/routing).

See the [Startup Tasks](Startup-Tasks) documentation for information on creating your own startup tasks.

## Advanced

### Customizing the Startup Process

Many startup tasks include their own insertion points that allow you to include your own registrations e.g. `IRouteRegistration` for [registering MVC routes](/content-management/routing) or `IResourceFileProviderRegistration` for adding extra file providers to the view engine.

Additionally each startup task tends to use an injectable initializer that can be overridden using the [Dependency Injection](dependency-injection) system such as `IJsonSerializerSettingsFactory` or `IStaticFileOptionsConfiguration`.

As a last resort we also provide a configuration option that lets you filter the startup task collection in any way you choose:

```csharp
// ..other app builder code ommited

app.UseCofoundry(c =>
{
    c.StartupTaskFilter = startupTasks =>
    {
        return startupTasks.Where(t => t is not JsonConverterStartupConfigurationTask);
    };
});
```

### AssemblyDiscoveryRules

To support plugins and automatic dependency registration Cofoundry has to scan your applications assemblies at startup. To reduce the impact of scanning for types we only pick out assemblies we think will contain plugins and Cofoundry types. This selection process is based on discovery rules, with the [default rule set](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Web/App_Start/AssemblyPartsDiscovery/Rules/CofoundryAssemblyDiscoveryRule.cs) picking up assemblies with a name that either:

- **Starts with "Cofoundry",** e.g. "Cofoundry", "Cofoundry.Domain", "Cofoundry.Admin"
- **Starts with your applications assembly name,** e.g. "MyApplication", "MyApplication.Domain", "MyApplication.Data"
- **Includes the word "Plugin" after the start,** e.g. "Cofoundry.Plugin.SiteMap", "MyLib.Plugin.Helpers"

If this ruleset is too restrictive, you can add or replace rules during application startup:

**ExampleAssemblyDiscoveryRule.cs**

```csharp
using Microsoft.Extensions.DependencyModel;
using Cofoundry.Web;

public class ExampleAssemblyDiscoveryRule : IAssemblyDiscoveryRule
{
    public bool CanInclude(RuntimeLibrary libraryToCheck, IAssemblyDiscoveryRuleContext context)
    {
        return libraryToCheck.Name == "MyCompany.CofoundryHelpers";
    }
}
```

**Startup.cs**

```csharp
// ..other app builder code ommited

builder.Services
    .AddMvc()
    .AddCofoundry(builder.Configuration, c =>
    {
        c.AssemblyDiscoveryRules.Add(new ExampleAssemblyDiscoveryRule());
    });
```
