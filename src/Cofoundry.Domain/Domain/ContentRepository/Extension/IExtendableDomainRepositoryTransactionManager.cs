using Cofoundry.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Extendable
{
    public interface IExtendableDomainRepositoryTransactionManager : IDomainRepositoryTransactionManager
    {
        /// <summary>
        /// TransactionScopeManager instance only to be used for
        /// extending the transaction manager with extension methods
        /// </summary>
        ITransactionScopeManager TransactionScopeManager { get; }

        /// <summary>
        /// DbContext that can be used to apply TransactionScopeManager
        /// method to. This is only to be used for extending the transaction 
        /// manager with extension methods
        /// </summary>
        DbContext DbContext { get; }
    }
}
