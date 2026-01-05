namespace Cofoundry.Core.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IContainerRegister"/>.
/// </summary>
public static class ContainerRegisterExtensions
{
    private static readonly RegistrationOptions _scopedOptions = RegistrationOptions.Scoped();
    private static readonly RegistrationOptions _singletonOptions = RegistrationOptions.SingletonScope();

    extension(IContainerRegister containerRegister)
    {
        /// <summary>
        /// Registers a service as its concrete type only, using InstanceLifetime.Scoped.
        /// </summary>
        /// <typeparam name="TConcrete">Type to register.</typeparam>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        public IContainerRegister RegisterScoped<TConcrete>()
            where TConcrete : notnull
        {
            return containerRegister.Register<TConcrete>(_scopedOptions);
        }

        /// <summary>
        /// Registers a service using InstanceLifetime.Scoped.
        /// </summary>
        /// <typeparam name="TRegisterAs">Type to register the service as.</typeparam>
        /// <typeparam name="TConcrete">Concrete type to register.</typeparam>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        public IContainerRegister RegisterScoped<TRegisterAs, TConcrete>()
            where TConcrete : notnull, TRegisterAs
        {
            return containerRegister.Register<TRegisterAs, TConcrete>(_scopedOptions);
        }

        /// <summary>
        /// Registers a service instance using InstanceLifetime.Singleton.
        /// </summary>
        /// <typeparam name="TConcrete">Concrete type to register.</typeparam>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        public IContainerRegister RegisterSingleton<TConcrete>()
            where TConcrete : notnull
        {
            return containerRegister.Register<TConcrete>(_singletonOptions);
        }

        /// <summary>
        /// Registers a service instance using InstanceLifetime.Singleton.
        /// </summary>
        /// <typeparam name="TRegisterAs">Type to register the service as.</typeparam>
        /// <typeparam name="TConcrete">Concrete type to register.</typeparam>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        public IContainerRegister RegisterSingleton<TRegisterAs, TConcrete>()
            where TConcrete : notnull, TRegisterAs
        {
            return containerRegister.Register<TRegisterAs, TConcrete>(_singletonOptions);
        }
    }
}
