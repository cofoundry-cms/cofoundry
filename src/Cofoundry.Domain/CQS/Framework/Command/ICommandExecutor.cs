﻿namespace Cofoundry.Domain.CQS;

/// <summary>
/// Service for executing commands of various types.
/// </summary>
/// <remarks>
/// CQS design influenced by http://www.cuttingedge.it/blogs/steven/pivot/entry.php?id=91
/// </remarks>
public interface ICommandExecutor
{
    /// <summary>
    /// Handles the execution of the specified <paramref name="command"/>.
    /// </summary>
    /// <param name="command">Command to execute.</param>
    Task ExecuteAsync(ICommand command);

    /// <summary>
    /// Handles the execution of the specified <paramref name="command"/>.
    /// </summary>
    /// <param name="command">Command to execute.</param>
    /// <param name="executionContext">
    /// Optional custom execution context which can be used to impersonate/elevate permissions 
    /// or change the execution date.
    /// </param>
    Task ExecuteAsync(ICommand command, IExecutionContext? executionContext);

    /// <summary>
    /// Handles the execution of the specified <paramref name="command"/>.
    /// </summary>
    /// <param name="command">Command to execute.</param>
    /// <param name="userContext">
    /// Optional user context which can be used to impersonate/elevate permissions.
    /// </param>
    Task ExecuteAsync(ICommand command, IUserContext userContext);
}
