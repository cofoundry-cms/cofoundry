using System.Data.Common;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Core.Data.Internal;

/// <summary>
/// Default implementation of <see cref="IDefaultTransactionScopeManager"/>.
/// </summary>
/// <remarks>
/// This default implementation uses System.Transaction.TransactionScope internally
/// to manage transaction scopes. Prior to .NET Core 2.0 this used EF transactions
/// because System.Transactions was not available.
/// </remarks>
public sealed class DefaultTransactionScopeManager : IDefaultTransactionScopeManager
{
    private readonly Dictionary<int, PrimaryTransactionScope> _primaryTransactionScopes = [];
    private readonly ITransactionScopeFactory _transactionScopeFactory;

    public DefaultTransactionScopeManager(
        ITransactionScopeFactory transactionScopeFactory
        )
    {
        _transactionScopeFactory = transactionScopeFactory;
    }

    /// <inheritdoc/>
    public ITransactionScope Create(DbConnection dbConnection)
    {
        return Create(dbConnection, CreateScopeFactory());
    }

    /// <inheritdoc/>
    public ITransactionScope Create(DbConnection dbConnection,
            TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted
        )
    {
        var transactionScopeFactory = CreateScopeFactory(transactionScopeOption, isolationLevel);
        return Create(dbConnection, transactionScopeFactory);
    }

    /// <inheritdoc/>
    public ITransactionScope Create(DbConnection dbConnection, Func<TransactionScope> transactionScopeFactory)
    {
        ArgumentNullException.ThrowIfNull(dbConnection);
        ArgumentNullException.ThrowIfNull(transactionScopeFactory);

        ITransactionScope scope;
        var connectionHash = dbConnection.GetHashCode();
        var primaryScope = _primaryTransactionScopes.GetValueOrDefault(connectionHash);

        if (primaryScope == null)
        {
            primaryScope = new PrimaryTransactionScope(this, transactionScopeFactory);
            _primaryTransactionScopes.Add(connectionHash, primaryScope);
            scope = primaryScope;
        }
        else
        {
            scope = new ChildTransactionScope(primaryScope, transactionScopeFactory);
        }

        return scope;
    }

    /// <inheritdoc/>
    public ITransactionScope Create(DbContext dbContext)
    {
        var connection = dbContext.Database.GetDbConnection();

        return Create(connection);
    }

    /// <inheritdoc/>
    public ITransactionScope Create(DbContext dbContext, TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        var connection = dbContext.Database.GetDbConnection();

        return Create(connection, transactionScopeOption, isolationLevel);
    }

    /// <inheritdoc/>
    public ITransactionScope Create(DbContext dbContext, Func<TransactionScope> transactionScopeFactory)
    {
        var connection = dbContext.Database.GetDbConnection();

        return Create(connection, transactionScopeFactory);
    }

    /// <inheritdoc/>
    public void QueueCompletionTask(DbConnection dbConnection, Action actionToQueue)
    {
        var connectionHash = dbConnection.GetHashCode();
        var scope = _primaryTransactionScopes.GetValueOrDefault(connectionHash);

        // No scope, execute immediately
        if (scope == null)
        {
            actionToQueue();
        }
        else
        {
            scope.QueueCompletionTask(actionToQueue);
        }
    }

    /// <inheritdoc/>
    public Task QueueCompletionTaskAsync(DbConnection dbConnection, Func<Task> actionToQueue)
    {
        var connectionHash = dbConnection.GetHashCode();
        var scope = _primaryTransactionScopes.GetValueOrDefault(connectionHash);

        // No scope, execute immediately
        if (scope == null)
        {
            return actionToQueue();
        }

        scope.QueueCompletionTask(actionToQueue);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void QueueCompletionTask(DbContext dbContext, Action actionToQueue)
    {
        var dbConnection = dbContext.Database.GetDbConnection();
        QueueCompletionTask(dbConnection, actionToQueue);
    }

    /// <inheritdoc/>
    public Task QueueCompletionTaskAsync(DbContext dbContext, Func<Task> actionToQueue)
    {
        var dbConnection = dbContext.Database.GetDbConnection();
        return QueueCompletionTaskAsync(dbConnection, actionToQueue);
    }

    internal void DeregisterTransaction(PrimaryTransactionScope scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        var scopeToRemoveKey = _primaryTransactionScopes
            .Where(s => s.Value == scope)
            .Select(s => (int?)s.Key)
            .SingleOrDefault();

        if (scopeToRemoveKey.HasValue)
        {
            _primaryTransactionScopes.Remove(scopeToRemoveKey.Value);
        }
    }

    private Func<TransactionScope> CreateScopeFactory(
            TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted
        )
    {
        return () => _transactionScopeFactory.Create(
            transactionScopeOption,
            isolationLevel
        );
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        foreach (var scope in _primaryTransactionScopes)
        {
            scope.Value.Dispose();
        }

        _primaryTransactionScopes.Clear();
    }
}
