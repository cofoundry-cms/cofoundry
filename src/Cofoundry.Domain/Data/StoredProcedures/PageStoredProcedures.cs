using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Data access abstraction over stored procedures for page entities.
    /// </summary>
    public class PageStoredProcedures : IPageStoredProcedures
    {
        private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;
        private readonly CofoundryDbContext _dbContext;

        public PageStoredProcedures(
            IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor,
            CofoundryDbContext dbContext
            )
        {
            _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds a draft page version, copying all page blocks and other dependencies. This
        /// query autmatically updates the PagePublishStatusQuery lookup table.
        /// </summary>
        /// <param name="pageId">The id of the page to create the draft for.</param>
        /// <param name="copyFromPageVersionId">Optional id of the version to copy from, if null we copy from the latest published version.</param>
        /// <param name="createDate">Date to set as the create date for the new version.</param>
        /// <param name="creatorId">Id of the user who created the draft.</param>
        /// <returns>PageVersionId of the newly created draft version.</returns>
        public async Task<int> AddDraftAsync(
            int pageId,
            int? copyFromPageVersionId,
            DateTime createDate,
            int creatorId
            )
        {
            var newVersionId = await _entityFrameworkSqlExecutor
                .ExecuteCommandWithOutputAsync<int?>(_dbContext,
                "Cofoundry.Page_AddDraft",
                "PageVersionId",
                 new SqlParameter("PageId", pageId),
                 new SqlParameter("CopyFromPageVersionId", copyFromPageVersionId),
                 new SqlParameter("CreateDate", createDate),
                 new SqlParameter("CreatorId", creatorId)
                 );

            if (!newVersionId.HasValue)
            {
                throw new UnexpectedSqlStoredProcedureResultException("Cofoundry.Page_AddDraft", "No version id was returned.");
            }

            return newVersionId.Value;
        }

        /// <summary>
        /// Updates the PagePublishStatusQuery lookup table for all 
        /// PublishStatusQuery values.
        /// </summary>
        /// <param name="pageId">Id of the page to update.</param>
        public Task UpdatePublishStatusQueryLookupAsync(int pageId)
        {
            return _entityFrameworkSqlExecutor
                .ExecuteCommandAsync(_dbContext,
                "Cofoundry.Cofoundry.PagePublishStatusQuery_Update",
                 new SqlParameter("PageId", pageId)
                 );
        }
    }
}
