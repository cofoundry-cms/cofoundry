using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple factory for creating generic DomainRepositoryQueryContext
    /// instances.
    /// </summary>
    public static class DomainRepositoryQueryContextFactory
    {
        /// <summary>
        /// Simple creator function to make it simpler to create new
        /// instances, as you would have to specify the generic parameter
        /// with the standard constructor.
        /// </summary>
        public static DomainRepositoryQueryContext<TResult> Create<TResult>(
            IQuery<TResult> query,
            IExtendableContentRepository extendableRepository
            )
        {
            return new DomainRepositoryQueryContext<TResult>(query, extendableRepository);
        }
    }
}
