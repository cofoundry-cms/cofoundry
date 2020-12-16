using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IContentRespository extension root for the ImageAsset entity.
    /// </summary>
    public interface IContentRepositoryPageDirectoryRepository
    {
        #region queries

        /// <summary>
        /// Query a single page directory by it's database id.
        /// </summary>
        /// <param name="pageDirectoryId">Id of the directory to get.</param>
        IContentRepositoryPageDirectoryByIdQueryBuilder GetById(int pageDirectoryId);

        /// <summary>
        /// Queries that return all page directories.
        /// </summary>
        IContentRepositoryPageDirectoryGetAllQueryBuilder GetAll();

        #endregion
    }
}
