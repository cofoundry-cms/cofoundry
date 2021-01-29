using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for custom entity definitions.
    /// </summary>
    public interface IContentRepositoryCustomEntityDefinitionsRepository
    {
        /// <summary>
        /// Get a custom entity definition by it's unique 6 character code.
        /// </summary>
        /// <param name="customEntityDefinitionCode">Unique 6 letter code representing the entity.</param>
        IContentRepositoryCustomEntityDefinitionByCodeQueryBuilder GetByCode(string customEntityDefinitioncode);

        /// <summary>
        /// Get all custom entity definitions.
        /// </summary>
        IContentRepositoryCustomEntityDefinitionGetAllQueryBuilder GetAll();
    }
}
