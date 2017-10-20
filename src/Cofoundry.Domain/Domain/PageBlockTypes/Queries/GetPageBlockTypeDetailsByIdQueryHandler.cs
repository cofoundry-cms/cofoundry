using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageBlockTypeDetailsByIdQueryHandler
        : IAsyncQueryHandler<GetByIdQuery<PageBlockTypeDetails>, PageBlockTypeDetails>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<PageBlockTypeDetails>, PageBlockTypeDetails>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageBlockTypeDetailsMapper _pageBlockTypeDetailsMapper;

        public GetPageBlockTypeDetailsByIdQueryHandler(
            CofoundryDbContext dbContext,
            IQueryExecutor queryExecutor,
            IPageBlockTypeDetailsMapper pageBlockTypeDetailsMapper
            )
        {
            _queryExecutor = queryExecutor;
            _dbContext = dbContext;
            _pageBlockTypeDetailsMapper = pageBlockTypeDetailsMapper;
        }

        #endregion

        #region execution

        public async Task<PageBlockTypeDetails> ExecuteAsync(GetByIdQuery<PageBlockTypeDetails> query, IExecutionContext executionContext)
        {
            var blockTypeSummary = await GetPageBlockTypeById(query.Id);
            if (blockTypeSummary == null) return null;

            var result = _pageBlockTypeDetailsMapper.Map(blockTypeSummary);

            return result;
        }

        private async Task<PageBlockTypeSummary> GetPageBlockTypeById(int id)
        {
            var allBlockTypes = await _queryExecutor.GetAllAsync<PageBlockTypeSummary>();

            var blockTypeTypeSummary = allBlockTypes
                .SingleOrDefault(t => t.PageBlockTypeId == id);

            return blockTypeTypeSummary;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<PageBlockTypeDetails> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
