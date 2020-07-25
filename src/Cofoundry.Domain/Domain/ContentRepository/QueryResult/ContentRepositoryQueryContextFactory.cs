using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple factory for creating generic ContentRepositoryQueryContext
    /// instances.
    /// </summary>
    public static class ContentRepositoryQueryContextFactory
    {
        /// <summary>
        /// Simple creator function to make it simpler to create new
        /// instances, as you would have to specify the generic parameter
        /// with the standard constructor.
        /// </summary>
        public static ContentRepositoryQueryContext<TResult> Create<TResult>(
            IQuery<TResult> query,
            IExtendableContentRepository contentRepository
            )
        {
            return new ContentRepositoryQueryContext<TResult>(query, contentRepository);
        }
    }
}
