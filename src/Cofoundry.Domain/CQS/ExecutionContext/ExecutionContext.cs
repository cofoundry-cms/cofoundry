namespace Cofoundry.Domain.CQS.Internal;

/// <summary>
/// A snapshot of the context in which a Command or Query should be executed.
/// </summary>
public class ExecutionContext : IExecutionContext
{
    /// <inheritdoc />
    public IUserContext UserContext { get; set; } = Cofoundry.Domain.UserContext.Empty;

    /// <inheritdoc />
    public DateTime ExecutionDate { get; set; }

    /// <summary>
    /// A placeholder value to use for not-nullable values that you
    /// know will be initialized in later code. This value should not
    /// be used in data post-initialization.
    /// </summary>
    public static readonly ExecutionContext Uninitialized = new();
}
