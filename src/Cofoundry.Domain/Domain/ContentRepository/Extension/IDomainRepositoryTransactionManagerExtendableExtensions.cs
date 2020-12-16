using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Extendable
{
    public static class IDomainRepositoryTransactionManagerExtendableExtensions
    {
        public static IExtendableDomainRepositoryTransactionManager AsExtendableDomainRepositoryTransactionManager(this IDomainRepositoryTransactionManager domainRepositoryTransactionManager)
        {
            if (domainRepositoryTransactionManager is IExtendableDomainRepositoryTransactionManager extendable)
            {
                return extendable;
            }

            throw new Exception($"An {nameof(IDomainRepositoryTransactionManager)} implementation should also implement {nameof(IExtendableDomainRepositoryTransactionManager)} to allow internal/plugin extendibility.");
        }
    }
}
