using Cofoundry.Core.EntityFramework;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <inheritdoc/>
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

        public async Task<int> AddDraftAsync(
            int pageId,
            int? copyFromPageVersionId,
            DateTime createDate,
            int creatorId
            )
        {
            const string SP_NAME = "Cofoundry.Page_AddDraft";

            int? newVersionId;

            try
            {
                newVersionId = await _entityFrameworkSqlExecutor
                    .ExecuteCommandWithOutputAsync<int?>(_dbContext,
                    SP_NAME,
                    "PageVersionId",
                     new SqlParameter("PageId", pageId),
                     new SqlParameter("CopyFromPageVersionId", copyFromPageVersionId),
                     new SqlParameter("CreateDate", createDate),
                     new SqlParameter("CreatorId", creatorId)
                     );
            }
            catch (SqlException ex) when (ex.Number == StoredProcedureErrorNumbers.Page_AddDraft.DraftAlreadyExists)
            {
                throw new StoredProcedureExecutionException(ex);
            }
            catch (SqlException ex) when (ex.Number == StoredProcedureErrorNumbers.Page_AddDraft.NoVersionFoundToCopyFrom)
            {
                throw new StoredProcedureExecutionException(ex);
            }

            if (!newVersionId.HasValue)
            {
                throw new UnexpectedStoredProcedureResultException(SP_NAME, "No version id was returned.");
            }

            return newVersionId.Value;
        }

        public Task CopyBlocksToDraftAsync(
            int copyToPageId,
            int copyFromPageVersionId,
            DateTime createDate,
            int creatorId
            )
        {
            return _entityFrameworkSqlExecutor
                .ExecuteCommandAsync(_dbContext,
                "Cofoundry.Page_CopyBlocksToDraft",
                 new SqlParameter("PageId", copyToPageId),
                 new SqlParameter("CopyFromPageVersionId", copyFromPageVersionId),
                 new SqlParameter("CreateDate", createDate),
                 new SqlParameter("CreatorId", creatorId)
                 );
        }

        public Task UpdatePublishStatusQueryLookupAsync(int pageId)
        {
            return _entityFrameworkSqlExecutor
                .ExecuteCommandAsync(_dbContext,
                "Cofoundry.PagePublishStatusQuery_Update",
                 new SqlParameter("PageId", pageId)
                 );
        }
    }
}
