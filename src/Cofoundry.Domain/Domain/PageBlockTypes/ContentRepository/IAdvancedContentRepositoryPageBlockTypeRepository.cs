using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IAdvancedContentRespository extension root for the PageBlockType entity.
    /// </summary>
    public interface IAdvancedContentRepositoryPageBlockTypeRepository
    {
        #region queries

        /// <summary>
        /// Gets a block type by it's unique database id.
        /// </summary>
        /// <param name="pageBlockTypeId">
        /// Id of the block type to find.
        /// </param>
        IContentRepositoryPageBlockTypeByIdQueryBuilder GetById(int pageBlockTypeId);

        /// <summary>
        /// Gets all page block types registered in the system.
        /// </summary>
        IContentRepositoryPageBlockTypeGetAllQueryBuilder GetAll();

        #endregion
    }
}
