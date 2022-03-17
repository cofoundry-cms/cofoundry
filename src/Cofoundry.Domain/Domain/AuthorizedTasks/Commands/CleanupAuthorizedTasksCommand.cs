namespace Cofoundry.Domain;

/// <summary>
/// Removes completed, invalid or expired tasks from the database after a 
/// period of time.
/// </summary>
public class CleanupAuthorizedTasksCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// The amount of time to keep completed, invalid or expired items in the database.
    /// Defaults to 30 days.
    /// </summary>
    public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(30);
}
