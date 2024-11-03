Cofoundry relies heavily on dependency injection, building on top of the [.NET Core dependency abtractions](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) to provide additional features such as:

- Automatic registrations
- Modular dependency registrations
- Overriding existing implementations

These features are essential for Cofoundry's modular framework to work, and you may find benefit in using them structure your code, however, for most scenarios you can continue to use the .NET Core dependency framework if you prefer.

## Automatic Registrations

Most Cofoundry services will automatically register themselves with the DI container, so you won't need to worry about registering types such as:

- Command & Query Handlers
- Custom Entity Definitions
- Background Tasks
- Startup Tasks
- User Areas
- Permissions
- "Registration" implementations (e.g. IRouteRegistration & IStartupTask)
- Plus many more

In fact, for some applications you may not need to worry about dependencies at all.

## Modular Registrations

Cofoundry provides an abstraction for registering dependencies that it uses internally, but you can use it too if you want to take advantage of a standardized approach to modular registrations. This is also essential for allowing plugin developers to register types and override base level implementations.

To register a type, all you need to do is create a class that implements `IDependencyRegistration` and add your registrations in the `Register` method. Your types will automatically be registered at startup. You can implement as many of these registration classes as you like which can be handy for keeping your registrations and code modular.

```csharp
using Cofoundry.Core.DependencyInjection;

public class ExampleRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container
            .Register<IMyService, MyService>()
            .RegisterSingleton<ExampleHelper>()
            .RegisterAll<IExampleTask>()
            .RegisterGeneric(typeof(IRepository<>), typeof(Repository<>));
    }
}
```

#### Overriding Registrations

Most registration methods have an optional `RegistrationOptions` parameter which allows you to override an existing implementation. This is useful (particularly for plugin authors) if you want to override the base Cofoundry implementation with your own version.

```csharp
using Cofoundry.Core.DependencyInjection;

public class ExampleOverrideRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        var registrationOptions = new RegistrationOptions() { ReplaceExisting = true };
        container.Register<IMyService, MyService>(registrationOptions);

        // OR use the static helper

        container.Register<IMyService, MyService>(RegistrationOptions.Override());
    }
}
```

`RegistrationOptions` has an additional priority property, but this should not normally be needed and should only be used as a last resort. The property is an integer but the `RegistrationOverridePriority` enum is best used for predictable results:

- **RegistrationOverridePriority.Low:** Will override the default implementation and nothing more. Typically used inside the Cofoundry framework to override a default/empty implementation lower down in the framework.
- **RegistrationOverridePriority.Normal:** Default and the option to typically use in a plugin. Will override the existing implementation and any low level (typically default/placeholder) implementation.
- **RegistrationOverridePriority.High:** A higher level priority that should rarely be used (and never in the framework or plugins), but may be needed in a client application to override a plugin.

## Registering Dependencies in other assemblies

Cofoundry uses a rule based system to determine which assemblies to scan for dependencies on startup. To keep the startup process lean, the default ruleset scans assemblies with names that match one the following criteria:

- Contains the text *"Cofoundry"*, e.g. `Cofoundry.Core` or `MyApp.CofoundryModule`
- Starts with the entry assembly name (typically your web app project name) e.g. `MyApp` or `MyApp.Domain`
- Contains the text *"Plugin"*, but not at the start, e.g. `Cofoundry.Plugins.Azure` or `MyApp.MyPlugin` 

#### Custom Discovery Rules

You can implement your own discovery rules by implementing `IAssemblyDiscoveryRule` and registering it with Cofoundry during application startup:

**MyAssemblyDiscoveryRule.cs**
```csharp
using Cofoundry.Web;
using Microsoft.Extensions.DependencyModel;

public class MyAssemblyDiscoveryRule : IAssemblyDiscoveryRule
{
    public bool CanInclude(
        RuntimeLibrary libraryToCheck, 
        IAssemblyDiscoveryRuleContext context
        )
    {
        return libraryToCheck.Name.StartsWith("MyNamespace.");
    }
}
```

**Program.cs**
```csharp
// other code removed

builder
    .Services
    .AddMvc()
    .AddCofoundry(builder.Configuration, c =>
    {
        c.AssemblyDiscoveryRules.Add(new MyAssemblyDiscoveryRule());
    });
```