using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain.CQS.Internal;

/// <summary>
/// Default implementation of <see cref="ICommandHandlerFactory"/>.
/// </summary>
public class CommandHandlerFactory : ICommandHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CommandHandlerFactory(
        IServiceProvider serviceProvider
        )
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public ICommandHandler<T> CreateAsyncHandler<T>() where T : ICommand
    {
        return _serviceProvider.GetRequiredService<ICommandHandler<T>>();
    }
}
