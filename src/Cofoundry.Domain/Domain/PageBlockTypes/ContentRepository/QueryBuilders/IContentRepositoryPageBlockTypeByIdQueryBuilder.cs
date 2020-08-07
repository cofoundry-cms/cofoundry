using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving user data for a unique database id.
    /// </summary>
    public interface IContentRepositoryPageBlockTypeByIdQueryBuilder
    {
        /// <summary>
        /// The PageBlockTypeSummary projection is lightweight and designed to be cacheable.
        /// The results of this query are cached by default.
        /// </summary>
        IDomainRepositoryQueryContext<PageBlockTypeSummary> AsSummary();

        /// <summary>
        /// The PageBlockTypeDetails projection extends the PageBlockTypeSummary model and
        /// contains additional data model schema meta data.
        /// </summary>
        IDomainRepositoryQueryContext<PageBlockTypeDetails> AsDetails();
    }
}
