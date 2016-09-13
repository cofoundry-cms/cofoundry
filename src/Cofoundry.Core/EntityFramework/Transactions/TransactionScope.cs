using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.EntityFramework
{
    /// <summary>
    /// A 'primary' or 'outer' transaction scope that directly represents
    /// the underlying transaction initiated by through EF.
    /// </summary>
    public class TransactionScope : ITransactionScope, IDisposable
    {
        private readonly DbContextTransaction _dbContextTransaction;
        private readonly TransactionScopeFactory _transactionScopeFactory;
        private TransactionState _transactionState = TransactionState.Open;

        public TransactionScope(
            TransactionScopeFactory transactionScopeFactory,
            DbContextTransaction dbContextTransaction
            )
        {
            _dbContextTransaction = dbContextTransaction;
            _transactionScopeFactory = transactionScopeFactory;
        }

        /// <summary>
        /// Commits the underlying EF transaction
        /// </summary>
        public void Complete()
        {
            switch (_transactionState)
            {
                case TransactionState.Open:
                    _dbContextTransaction.Commit();
                    _transactionState = TransactionState.Commited;
                    break;
                case TransactionState.RolledBack:
                    throw new ChildTransactionRolledBackException("Cannot complete the transaction because the transaction has already been rolled back by a child scope");
                case TransactionState.Closed:
                case TransactionState.Commited:
                    throw new InvalidOperationException("Cannot complete the transaction because the transaction is already " + _transactionState.ToString().ToLower());
                default:
                    throw new NotImplementedException("Unrecognised TransactionState: " + _transactionState);
            }
        }

        /// <summary>
        /// Rolls back the transaction.
        /// </summary>
        /// <remarks>
        /// This is internal because it is only intended to be called by a ChildTransactionScope
        /// </remarks>
        internal void Rollback()
        {
            switch (_transactionState)
            {
                case TransactionState.Open:
                    _dbContextTransaction.Rollback();
                    _transactionState = TransactionState.RolledBack;
                    break;
                case TransactionState.RolledBack:
                    // The transaction has already been rolled back
                    break;
                case TransactionState.Closed:
                case TransactionState.Commited:
                    throw new InvalidOperationException("Cannot rollback the transaction because the transaction is already " + _transactionState.ToString().ToLower());
                default:
                    throw new NotImplementedException("Unrecognised TransactionState: " + _transactionState);
            }
        }

        public void Dispose()
        {
            if (_dbContextTransaction != null)
            {
                if (_transactionScopeFactory != null)
                {
                    // De-register this scope as the primary transaction so others can be created
                    _transactionScopeFactory.DeregisterTransaction(this);
                }
                // Dispose of the EF transaction which should tidy itself up.
                _dbContextTransaction.Dispose();
            }
        }
    }
}
