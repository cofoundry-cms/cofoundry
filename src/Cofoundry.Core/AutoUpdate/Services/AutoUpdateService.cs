using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Cofoundry.Core.AutoUpdate.Internal;

/// <summary>
/// Service to update applications and modules. Typically this is
/// run at application startup.
/// </summary>
public class AutoUpdateService : IAutoUpdateService
{
    private static readonly MethodInfo _runVersionedCommandMethod = typeof(AutoUpdateService).GetMethod(nameof(ExecuteGenericVersionedCommand), BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly MethodInfo _runAlwaysRunCommandMethod = typeof(AutoUpdateService).GetMethod(nameof(ExecuteGenericAlwaysRunCommand), BindingFlags.NonPublic | BindingFlags.Instance);

    private readonly IAutoUpdateStore _autoUpdateStore;
    private readonly IEnumerable<IUpdatePackageFactory> _updatePackageFactories;
    private readonly IEnumerable<IStartupValidator> _startupValidators;
    private readonly IUpdateCommandHandlerFactory _commandHandlerFactory;
    private readonly IUpdatePackageOrderer _updatePackageOrderer;
    private readonly AutoUpdateSettings _autoUpdateSettings;
    private readonly IAutoUpdateDistributedLockManager _autoUpdateDistributedLockManager;
    private readonly ILogger<AutoUpdateService> _logger;

    public AutoUpdateService(
        IAutoUpdateStore autoUpdateStore,
        IEnumerable<IUpdatePackageFactory> updatePackageFactories,
        IEnumerable<IStartupValidator> startupValidators,
        IUpdateCommandHandlerFactory commandHandlerFactory,
        IUpdatePackageOrderer updatePackageOrderer,
        AutoUpdateSettings autoUpdateSettings,
        IAutoUpdateDistributedLockManager autoUpdateDistributedLockManager,
        ILogger<AutoUpdateService> logger
        )
    {
        _autoUpdateStore = autoUpdateStore;
        _updatePackageFactories = updatePackageFactories;
        _startupValidators = startupValidators;
        _commandHandlerFactory = commandHandlerFactory;
        _updatePackageOrderer = updatePackageOrderer;
        _autoUpdateSettings = autoUpdateSettings;
        _autoUpdateDistributedLockManager = autoUpdateDistributedLockManager;
        _logger = logger;
    }

    public async Task UpdateAsync(CancellationToken? cancellationToken = null)
    {
        RunStartupValidation();

        var packages = await GetOrderedPackages();
        if (!packages.Any())
        {
            _logger.LogTrace("No update packages found.");
            return;
        }

        var isLocked = await IsLockedAsync();
        if (isLocked && !packages.Any(p => p.ContainsVersionUpdates()))
        {
            // if locked ignore always-update commands and only throw if there
            // are required version updates.
            return;
        }
        else if (isLocked)
        {
            throw new DatabaseLockedException();
        }

        if (IsCancelled(cancellationToken)) return;

        await RunUpdates(packages, cancellationToken);
    }

    private void RunStartupValidation()
    {
        // run these prior to taking a lock to avoid any
        // developer exceptions causing an orphaned lock
        foreach (var startupValidator in _startupValidators)
        {
            startupValidator.Validate();
        }
    }

    public async Task<bool> IsLockedAsync()
    {
        if (_autoUpdateSettings.Disabled) return true;

        return await _autoUpdateStore.IsDatabaseLockedAsync();
    }

    public Task SetLockedAsync(bool isLocked)
    {
        return _autoUpdateStore.SetDatabaseLockedAsync(isLocked);
    }

    private async Task<ICollection<UpdatePackage>> GetOrderedPackages()
    {
        var previouslyAppliedVersions = await _autoUpdateStore.GetVersionHistoryAsync();

        var filteredPackages = _updatePackageFactories
            .SelectMany(f => f.Create(previouslyAppliedVersions))
            .ToList();

        var packages = _updatePackageOrderer.Order(filteredPackages);

        return packages;
    }

    private bool IsCancelled(CancellationToken? cancellationToken)
    {
        _logger.LogTrace("Cancellation requested");

        return cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested;
    }

    private async Task RunUpdates(ICollection<UpdatePackage> packages, CancellationToken? cancellationToken)
    {
        // Lock the process to prevent concurrent updates
        var distributedLock = await _autoUpdateDistributedLockManager.LockAsync();

        try
        {
            foreach (var package in packages)
            {
                if (IsCancelled(cancellationToken))
                {
                    break;
                }

                await ExecutePackage(package, cancellationToken);
            }
        }
        catch (Exception updateProcessException)
        {
            // Try and close the lock before we throw the exception
            try
            {
                await _autoUpdateDistributedLockManager.UnlockAsync(distributedLock);
            }
            catch (Exception unlockException)
            {
                _logger.LogError(unlockException, unlockException.Message);
            }

            throw;
        }

        await _autoUpdateDistributedLockManager.UnlockAsync(distributedLock);
    }

    private async Task ExecutePackage(UpdatePackage package, CancellationToken? cancellationToken)
    {
        _logger.LogDebug("Executing module package {ModuleIdentifier}.", package.ModuleIdentifier);

        // Versioned Commands
        foreach (var command in EnumerableHelper.Enumerate(package.VersionedCommands))
        {
            if (IsCancelled(cancellationToken))
            {
                // If cancelled, don't try the next version package, but do try and run the always run commands.
                break;
            }

            _logger.LogDebug("Executing version {Version} command '{Description}'.", command.Version, command.Description);

            try
            {
                await ExecuteCommandAsync(command);
            }
            catch (Exception ex)
            {
                await LogUpdateErrorAsync(package.ModuleIdentifier, command.Version, command.Description, ex);
                throw;
            }

            await _autoUpdateStore.LogSuccessAsync(package.ModuleIdentifier, command.Version, command.Description);
        }

        // Always Run Commands
        foreach (var command in EnumerableHelper.Enumerate(package.AlwaysUpdateCommands))
        {
            _logger.LogDebug("Executing always run command '{Description}'.", command.Description);
            await ExecuteCommandAsync(command);
        }
    }

    private async Task LogUpdateErrorAsync(string module, int version, string description, Exception ex)
    {
        try
        {
            await _autoUpdateStore.LogErrorAsync(module, version, description, ex);
        }
        catch (Exception loggingException)
        {
            throw new AutoUpdateErrorLoggingException(ex, loggingException);
        }
    }

    private Task ExecuteCommandAsync(IVersionedUpdateCommand command)
    {
        var task = (Task)_runVersionedCommandMethod
            .MakeGenericMethod(command.GetType())
            .Invoke(this, new object[] { command });

        return task;
    }

    private Task ExecuteCommandAsync(IAlwaysRunUpdateCommand command)
    {
        var task = (Task)_runAlwaysRunCommandMethod
            .MakeGenericMethod(command.GetType())
            .Invoke(this, new object[] { command });

        return task;
    }

    private Task ExecuteGenericVersionedCommand<TCommand>(TCommand command) where TCommand : IVersionedUpdateCommand
    {
        var runner = _commandHandlerFactory.CreateVersionedCommand<TCommand>();

        if (runner is IAsyncVersionedUpdateCommandHandler<TCommand>)
        {
            return ((IAsyncVersionedUpdateCommandHandler<TCommand>)runner).ExecuteAsync(command);
        }
        else
        {
            ((ISyncVersionedUpdateCommandHandler<TCommand>)runner).Execute(command);
            return Task.CompletedTask;
        }
    }

    private Task ExecuteGenericAlwaysRunCommand<TCommand>(TCommand command) where TCommand : IAlwaysRunUpdateCommand
    {
        var runner = _commandHandlerFactory.CreateAlwaysRunCommand<TCommand>();

        if (runner is IAsyncAlwaysRunUpdateCommandHandler<TCommand>)
        {
            return ((IAsyncAlwaysRunUpdateCommandHandler<TCommand>)runner).ExecuteAsync(command);
        }
        else
        {
            ((ISyncAlwaysRunUpdateCommandHandler<TCommand>)runner).Execute(command);
            return Task.CompletedTask;
        }
    }
}
