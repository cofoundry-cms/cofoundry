using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.EntityFramework
{
    /// <summary>
    /// An abstraction over EF transactions which allows for nested transaction scopes.
    /// </summary>
    /// <remarks>
    /// Why not use TransactionScope? There are issues with using TransactionScope and 
    /// EF async and also it is not supported in .Net Core.
    /// </remarks>
    public interface ITransactionScopeFactory : IDisposable
    {
        /// <summary>
        /// Creates a new transaction scope. The scope can be nested inside another scope
        /// in which case the underlying db transaction is only commited once both the outer
        /// and inner transaction(s) have been committed. The returned ITransactionScope 
        /// implements IDisposable and should be wrapped in a using statement.
        /// </summary>
        /// <returns>ITransactionScope, which is IDisposable and must be disposed.</returns>
        ITransactionScope Create(DbContext dbContext);
    }
}
