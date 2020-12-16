using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetDocumentAssetRenderDetailsByIdRangeQueryHandler
        : IQueryHandler<GetDocumentAssetRenderDetailsByIdRangeQuery, IDictionary<int, DocumentAssetRenderDetails>>
        , IPermissionRestrictedQueryHandler<GetDocumentAssetRenderDetailsByIdRangeQuery, IDictionary<int, DocumentAssetRenderDetails>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IDocumentAssetRenderDetailsMapper _documentAssetRenderDetailsMapper;

        public GetDocumentAssetRenderDetailsByIdRangeQueryHandler(
            CofoundryDbContext dbContext,
            IDocumentAssetRenderDetailsMapper documentAssetRenderDetailsMapper
            )
        {
            _dbContext = dbContext;
            _documentAssetRenderDetailsMapper = documentAssetRenderDetailsMapper;
        }

        #endregion

        #region execution

        public async Task<IDictionary<int, DocumentAssetRenderDetails>> ExecuteAsync(GetDocumentAssetRenderDetailsByIdRangeQuery query, IExecutionContext executionContext)
        {
            var dbResults = await QueryDb(query).ToListAsync();

            var mappedResults = dbResults
                .Select(_documentAssetRenderDetailsMapper.Map)
                .ToDictionary(d => d.DocumentAssetId);

            return mappedResults;
        }

        private IQueryable<DocumentAsset> QueryDb(GetDocumentAssetRenderDetailsByIdRangeQuery query)
        {
            return _dbContext
                .DocumentAssets
                .AsNoTracking()
                .FilterByIds(query.DocumentAssetIds);
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetDocumentAssetRenderDetailsByIdRangeQuery query)
        {
            yield return new DocumentAssetReadPermission();
        }

        #endregion
    }
}
