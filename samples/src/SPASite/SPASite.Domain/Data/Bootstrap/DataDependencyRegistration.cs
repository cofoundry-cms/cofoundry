using Cofoundry.Core.DependencyInjection;

namespace SPASite.Data;

/// <summary>
/// An IDependencyRegistration class allows us to automatically register 
/// services with the DI container in a modular way.
/// 
/// See https://www.cofoundry.org/docs/framework/dependency-injection
/// </summary>
public class DataDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        container.RegisterScoped<SPASiteDbContext>();
    }
}