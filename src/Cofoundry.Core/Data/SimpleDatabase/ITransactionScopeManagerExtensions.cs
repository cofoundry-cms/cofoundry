namespace Cofoundry.Core.Data.SimpleDatabase;

public static class ITransactionScopeManagerExtensions
{
    /// <summary>
    /// Creates a new transaction scope associated with the connection in use by the
    /// specified <see cref="IDatabase"/> instance. The scope can be nested inside another scope in which 
    /// case the underlying db transaction is only committed once both the outer
    /// and inner transaction(s) have been committed. The returned <see cref="ITransactionScope"/> 
    /// implements <see cref="IDisposable"/> and should be wrapped in a using statement.
    /// </summary>
    /// <param name="transactionScopeManager">
    /// Source <see cref="ITransactionScopeManager"/> instance to extend.
    /// </param>
    /// <param name="database">
    /// <para>
    /// The <see cref="IDatabase"/> instance to manage transactions for. Transaction scopes
    /// created by this instance only apply to a single DbConnection, so if you want 
    /// the scope to span multiple <see cref="IDatabase"/> instances then they must share the same 
    /// connection.
    /// </para>
    /// <para>
    /// You can use the <see cref="ICofoundryDbConnectionManager"/> to get a reference to the shared 
    /// connection directly.
    /// </para>
    /// </param>
    /// <returns>An <see cref="ITransactionScope"/> instance, which is <see cref="IDisposable"/> and must be disposed.</returns>
    public static ITransactionScope Create(this ITransactionScopeManager transactionScopeManager, IDatabase database)
    {
        return transactionScopeManager.Create(database.GetDbConnection());
    }
}
