using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Core.AutoUpdate;

/// <summary>
/// Factory that creates concrete implementations of IUpdateCommand handlers.
/// </summary>
public class UpdateCommandHandlerFactory : IUpdateCommandHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public UpdateCommandHandlerFactory(
        IServiceProvider serviceProvider
        )
    {
        _serviceProvider = serviceProvider;
    }

    public IVersionedUpdateCommandHandler<TCommand> CreateVersionedCommand<TCommand>() where TCommand : IVersionedUpdateCommand
    {
        var asyncCommand = _serviceProvider.GetService<IAsyncVersionedUpdateCommandHandler<TCommand>>();

        if (asyncCommand != null)
        {
            return asyncCommand;
        }
        else
        {
            return _serviceProvider.GetRequiredService<ISyncVersionedUpdateCommandHandler<TCommand>>();
        }
    }

    public IAlwaysRunUpdateCommandHandler<TCommand> CreateAlwaysRunCommand<TCommand>() where TCommand : IAlwaysRunUpdateCommand
    {
        var asyncCommand = _serviceProvider.GetService<IAsyncAlwaysRunUpdateCommandHandler<TCommand>>();

        if (asyncCommand != null)
        {
            return asyncCommand;
        }
        else
        {
            return _serviceProvider.GetRequiredService<ISyncAlwaysRunUpdateCommandHandler<TCommand>>();
        }
    }
}
