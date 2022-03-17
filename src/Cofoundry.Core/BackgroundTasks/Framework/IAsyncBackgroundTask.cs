namespace Cofoundry.Core.BackgroundTasks;

/// <summary>
/// Represents a task to execute 
/// </summary>
public interface IAsyncBackgroundTask
{
    Task ExecuteAsync();
}
