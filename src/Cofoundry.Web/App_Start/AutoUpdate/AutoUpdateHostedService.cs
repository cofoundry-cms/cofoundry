﻿using Cofoundry.Core.AutoUpdate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Web;

/// <summary>
/// A hosted service to run the auto-update process at startup.
/// This is done in a hosted service to allow the process to be
/// no-blocking while supporting re-tries without the timeout
/// restrictions of being executed in a request.
/// </summary>
public class AutoUpdateHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public AutoUpdateHostedService(
        IServiceProvider serviceProvider,
        ILogger<AutoUpdateHostedService> logger
        )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool isComplete = false;

        _logger.LogInformation("Starting service");
        uint numAttempts = 0;

        // The update is essential so we should keep re-trying until compelete
        while (!stoppingToken.IsCancellationRequested && !isComplete)
        {
            isComplete = await TryUpdate(stoppingToken);

            if (!isComplete)
            {
                var retryTimeout = GetRetryTimeoutInSeconds(numAttempts);

                // Use a short re-try delay to ensure we process the update quickly
                // without overwhelming server resources
                _logger.LogInformation("Process failed, retrying in {RetryTimeoutInSeconds} seconds", retryTimeout);
                await Task.Delay(TimeSpan.FromSeconds(retryTimeout), stoppingToken);

                numAttempts++;
            }
        }
    }

    private static int GetRetryTimeoutInSeconds(ulong numAttmpts)
    {
        return numAttmpts switch
        {
            > 30 => 60,
            > 20 => 30,
            > 10 => 15,
            > 2 => 5,
            _ => 1
        };
    }

    private async Task<bool> TryUpdate(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        // state is a singleton that other startup services can use to check the state o fthe process
        var state = scope.ServiceProvider.GetRequiredService<AutoUpdateState>();

        if (state.Status == AutoUpdateStatus.Complete || state.Status == AutoUpdateStatus.InProgress)
        {
            throw new Exception("Unexpected initial auto-update state: " + state.Status);
        }

        state.Update(AutoUpdateStatus.InProgress);

        try
        {
            var autoUpdateService = scope.ServiceProvider.GetRequiredService<IAutoUpdateService>();
            await autoUpdateService.UpdateAsync(stoppingToken);

            state.Update(AutoUpdateStatus.Complete);
        }
        catch (AutoUpdateProcessLockedException)
        {
            state.Update(AutoUpdateStatus.LockedByAnotherProcess);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing update");
            state.Update(AutoUpdateStatus.Error, ex);
        }

        if (state.Status == AutoUpdateStatus.Complete)
        {
            _logger.LogInformation("Process completed");
            return true;
        }

        return false;
    }
}
